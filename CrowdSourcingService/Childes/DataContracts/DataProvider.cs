using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Childes.DataContracts;
using Childes.Extensions;
using Childes.ServiceContracts;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace Childes
{
    public class DataProvider : IDataProvider
    {
        private IDbConnection dbConnection;
        private IDbDataAdapter dbDataAdapter;

        private static string TABLE_TARGET = "target_to_context";

        public DataProvider(IDbConnection dbConnection, IDbDataAdapter dbDataAdapter)
        {
            this.dbConnection = dbConnection;
            this.dbDataAdapter = dbDataAdapter;
        }

        public IEnumerable<ITranscriptElement> GetNLeastAnnotated(int numberOfTasks)
        {
            Console.WriteLine("inside getnleastannotated");
            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                Console.WriteLine("command created");
                //this.dbDataAdapter.SelectCommand = this.dbConnection.CreateCommand();
                /*command.CommandText = string.Format(
                    "SELECT * FROM {0} ORDER BY completion_count ASC LIMIT {1}",
                    TABLE_TARGET, numberOfTasks);
                */
                command.CommandText = string.Format(
                    "SELECT * FROM {0} WHERE target_utterance >= 1857878 and target_utterance <= 1860126",
                    TABLE_TARGET);
                List<ITranscriptElement> targets = new List<ITranscriptElement>();
                Console.WriteLine("attempting to open connection");
                this.dbConnection.Open();
                Console.WriteLine("opened connection");
                try
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("reader executing");
                        while (reader.Read())
                        {
                            Utterance utterance = new Utterance();
                            this.PopulateChildesData(utterance, reader);

                            targets.Add(utterance);
                        }
                    }
                }
                finally
                {
                    Console.WriteLine("attempting to close");
                    this.dbConnection.Close();
                    Console.WriteLine("connection closed");
                }
                Console.WriteLine(targets);
                return targets;
            }

        }

        public IEnumerable<ITranscriptElement> GetPracticeTasks()
        {
            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                //this.dbDataAdapter.SelectCommand = this.dbConnection.CreateCommand();
                command.CommandText = string.Format(
                    "SELECT * FROM {0} WHERE `practice` = 'true'", TABLE_TARGET);

                List<ITranscriptElement> targets = new List<ITranscriptElement>();
                this.dbConnection.Open();
                try
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Utterance utterance = new Utterance();

                            //utterance.SetProperty("bucket_id", bucket.Id);
                            this.PopulateChildesData(utterance, reader);

                            targets.Add(utterance);
                        }
                    }
                }
                finally
                {
                    this.dbConnection.Close();
                }
                return targets;
            }
        }

        public IContextInfo GetContextInfo(ITranscriptElement element)
        {
            this.dbDataAdapter.SelectCommand = this.dbConnection.CreateCommand();
            this.dbDataAdapter.SelectCommand.CommandText = string.Format(
                "SELECT * FROM {0} WHERE target_utterance='{1}'",
                TABLE_TARGET, element.Id);

            ContextInfo contextInfo = null;

            this.dbConnection.Open();
            try
            {
                using (IDataReader reader = this.dbDataAdapter.SelectCommand.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        contextInfo = new ContextInfo();
                        this.PopulateChildesData(contextInfo, reader);
                    }
                }

                return contextInfo;
            }
            finally
            {
                this.dbConnection.Close();
            }
        }

        public IEnumerable<ITranscriptElement> GetContextContent(IContextInfo contextInfo)
        {
            List<Utterance> utterances = new List<Utterance>();

            string whereClause = null;
            string whereCondition = "`{0}` = '{1}'";
            string columnName = "id";

            foreach (string transcriptId in contextInfo.TranscriptIds)
            {
                if (string.IsNullOrWhiteSpace(whereClause))
                {
                    whereClause = string.Format(
                        "WHERE {0}",
                        string.Format(whereCondition, columnName, transcriptId));
                }
                else
                {
                    whereClause = string.Format(
                        "{0} OR {1}",
                        whereClause,
                        string.Format(whereCondition, columnName, transcriptId));
                }
            }

            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                command.CommandText = string.Format(
                    "SELECT * FROM all_utterances {0}",
                    whereClause);

                    
                this.dbConnection.Open();
                try
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Utterance utterance = new Utterance();
                            this.PopulateChildesData(utterance, reader);

                            utterances.Add(utterance);
                        }
                    }
                }
                finally
                {
                    this.dbConnection.Close();
                }
            }

            return utterances;
        }

        public bool AddWorker(string workerId)
        {
            //Step 1: Anonymize
            string hashedWorkerId = Anonymize(workerId);
            if (this.dbConnection == null)
            {
                throw new Exception();
            }

            if (!HasWorker(hashedWorkerId)) //user never landed on page OR user landed but never completed task
            {
                string annotatorTable = "annotators";

                using (IDbCommand command = this.dbConnection.CreateCommand())
                {
                    command.CommandText = string.Format(
                        "INSERT INTO `{0}`(`userName`) values(\"{1}\")" +
                        " ON DUPLICATE KEY UPDATE `userName`=\"{1}\"",
                        annotatorTable,
                        hashedWorkerId);

                    this.dbConnection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                        /*if (command.ExecuteNonQuery() == 0)
                        {
                            throw new Exception();
                        }*/
                    }
                    finally
                    {
                        this.dbConnection.Close();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddAnnotation(ITranscriptElement transcript, IPropertyBag annotation, string workerId, string assignmentId)
        {
            string hashedWorkerId = Anonymize(workerId);
            if (this.dbConnection == null)
            {
                throw new Exception();
            }

            if (!annotation.ValidateAsAnnotation())
            {
                throw new Exception("Invalid annotation");
            }

            string annotationTable = "annotations";
           
            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BsonWriter writer = new BsonWriter(ms))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(writer, annotation);
                    }
                    
                    command.CommandText = string.Format(
                        "INSERT INTO `{0}`(`target_utterance`, `annotation`, `annotator`, `assignment`) values(\"{1}\", \"{2}\", \"{3}\", \"{4}\")",
                        annotationTable,
                        transcript.Id,
                        Convert.ToBase64String(ms.ToArray()),
                        hashedWorkerId,
                        assignmentId);
                }
                
                this.dbConnection.Open();
                try
                {
                    if (command.ExecuteNonQuery() == 0)
                    {
                        throw new Exception();
                    }
                }
                finally
                {
                    this.dbConnection.Close();
                }
            }
            updateAnnotationNumber(transcript);
        }

        public void AddAnnotatorInformation(string annotator, IPropertyBag metadata)
        {
            string hashedAnnotator = Anonymize(annotator);
            if (this.dbConnection == null)
            {
                throw new Exception();
            }

            string annotatorTable = "annotators";

            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BsonWriter writer = new BsonWriter(ms))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(writer, metadata);
                    }

                    command.CommandText = string.Format(
                        "UPDATE `{0}` SET `metadata` = \"{1}\", `completed`=\"true\" WHERE `userName` = \"{2}\"",
                        annotatorTable,
                        Convert.ToBase64String(ms.ToArray()),
                        hashedAnnotator);
                }

                this.dbConnection.Open();
                try
                {
                    if (command.ExecuteNonQuery() == 0)
                    {
                        throw new Exception();
                    }
                }
                finally
                {
                    this.dbConnection.Close();
                }
            }
        }

        public void updateAnnotationNumber(ITranscriptElement transcript)
        {
            if (this.dbConnection == null)
            {
                throw new Exception();
            }
            object prevCountObj = "";
            int prevCount;
            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                command.CommandText = string.Format(
                    "SELECT `completion_count` FROM `{0}` WHERE `target_utterance` = \"{1}\"",
                    TABLE_TARGET,
                    transcript.Id);

                this.dbConnection.Open();

                try
                {
                    prevCountObj = command.ExecuteScalar();
                    if (prevCountObj != null)
                        int.TryParse(prevCountObj.ToString(), out prevCount);
                    else
                        prevCount = 0;
                }
                finally
                {
                    this.dbConnection.Close();
                }
            }
            


            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                command.CommandText = string.Format(
                        "UPDATE `{0}` SET `completion_count` = {1} WHERE `target_utterance` = \"{2}\"",
                        TABLE_TARGET,
                        prevCount + 1,
                        transcript.Id);

                this.dbConnection.Open();
                try
                {
                    if (command.ExecuteNonQuery() == 0)
                    {
                        throw new Exception();
                    }
                }
                finally
                {
                    this.dbConnection.Close();
                }
            }
        }

        public IMedia GetMedia(string videoId)
        {
            byte[] videoEncoded = null;
            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                command.CommandText = string.Format(
                    "SELECT `video` FROM `{0}` WHERE `id`=\"{1}\"",
                    "videos", videoId);

                this.dbConnection.Open();

                try
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        videoEncoded = (byte[])reader.GetValue(0);
                    }
                }
                finally
                {
                    this.dbConnection.Close();
                }
            }

            string encoded = Encoding.UTF8.GetString(videoEncoded);
            return new Video(encoded);
        }

        private void PopulateChildesData(ChildesData utterance, IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; ++i)
            {
                utterance.SetProperty(reader.GetName(i), reader.GetValue(i));
            }
        }

        public IPropertyBag GetAnnotation(string id)
        {
            if (this.dbConnection == null)
            {
                throw new Exception();
            }

            string annotationTable = "annotations";

            using (IDbCommand command = this.dbConnection.CreateCommand())
            {
                //add where practice is true later
                command.CommandText = string.Format(
                        "SELECT `annotation` FROM `{0}` WHERE `target_utterance` = {1} AND `practice` = 'true'",
                        annotationTable,
                        id);

                this.dbConnection.Open();
                Annotation correctAnno;
                try
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        byte[] correctByteArray = (byte[])reader.GetValue(0);
                        string base64BsonStr = Encoding.UTF8.GetString(correctByteArray);
                        using (Stream stream = new MemoryStream(Convert.FromBase64String(base64BsonStr)))
                        {
                            using (BsonReader br = new BsonReader(stream))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                correctAnno = serializer.Deserialize<Annotation>(br);
                            }
                        }
                        
                    }
                }
                finally
                {
                    this.dbConnection.Close();
                }
                return correctAnno;
            }
        }

        private bool HasWorker(string workerId)
        {
            this.dbDataAdapter.SelectCommand = this.dbConnection.CreateCommand();
            this.dbDataAdapter.SelectCommand.CommandText = string.Format(
                "SELECT completed FROM annotators WHERE userName='{0}'",
                workerId);

            this.dbConnection.Open();
            try
            {
                using (IDataReader reader = this.dbDataAdapter.SelectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (((string)reader.GetValue(0)).Equals("true"))
                        {
                            return true;
                        }
                    }
                }

               return false;
            }
            finally
            {
                this.dbConnection.Close();
            }
        }

        private string Anonymize(string workerId)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(workerId));
            foreach (byte b in crypto)
            {
                hash.Append(b.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}

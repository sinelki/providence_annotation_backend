# Providence Annotation Backend

This repo is one of several projects related to [CCLYC](https://github.com/sinelki/cclyc.git).
It is the service used to serve Annotation Tasks for
[Cues to Comparison Classes in Child-directed Language](http://library.mit.edu/F/PQKXE2YAGSC2MEUE92G1NESLJHRCHALE3ABDPS867K4HJBR97F-00503?func=file&amp=&amp=&amp=&amp=&amp=&amp=&file%5Fname=find-b&local%5Fbase=THESES2).
See Appendix C for additional information about the design of the service.

## Prerequisites<a href=”prerequisites”></a>

Download and install copies of the following software.

- [Docker](https://docs.docker.com/get-docker/)

If you haven’t already done so, follow the setup instructions for
[Providence Data Pipeline](https://github.com/sinelki/providence_data_pipeline#readme)

Ideally you will have access to a hosting environment (e.g. a cloud VM), however the
application can also be run locally.

## Setup<a href=”setup”></a>

1. Create a `dump.sql` file of the entire `providence_comparison_class2` database. 
1. Create the following *secrets* within the `data` folder. The content of each file
   should simply be the secret value itself.
   - db_root_password.env
   - db_user_name.env
   - db_user_password.env
   - db_connection_string.env
   - cert_passphrase.env

   e.g. 
   db_root_password.env
   ```
   Pa$sWorD12#
   ```

   > *Note*: The format of the db_connection_string.env should be
   > `server=<SOME IP>;port=<SOME PORT>;user=<DB USER>;password=<DB PASSWORD>; database=providence_comparison_class2; SslMode=none`
   > Simply replace all `<VALUES>` with the appropriate data.

1. Create an SSL certificate for the service image. (For testing purposes, you can run
   `make cert.pem` followed by `make certificate.pfx`)
1. Create the docker network for MySQL and service containers to communicate using
   `make create_docker_network`.
1. Run `make format_sql_dump` if you are using the MySQL5 docker container
1. Run MySQL docker container with `make run_sql_image`
1. Build service image with `make build_image`
1. Run service docker container with `make run_image`


## Development Setup<a href=development-setup></a>
To make any modifications, you will need to install the required development tools

- [.NET Core 2.1 ](https://dotnet.microsoft.com/download/dotnet-core)
- (Recommended)[Visual Studio](https://visualstudio.microsoft.com/) If running on Linux,
   you can download VSCode instead.

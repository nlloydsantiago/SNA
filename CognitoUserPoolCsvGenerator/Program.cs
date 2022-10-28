using System;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Serilog;

namespace CognitoUserPoolCsvGenerator;

public class Program
{
    static async Task Main(string[] args)
    {
        var logger = LoggerConfig.Configure();

        var authenticationDatabaseConnection = new SqlConnection(args[0]);
        logger.Information("SNA Authentication Database Connection: {authenticationDatabaseConnection}", authenticationDatabaseConnection.ConnectionString);

        var basicAwsCredentials = new BasicAWSCredentials(args[1], args[2]);
        var immutableCredentials = basicAwsCredentials.GetCredentials();
        logger.Information("BasicAWSCredentials accessKey: {accessKey} secretKey: {secretKey}",
            immutableCredentials.AccessKey, immutableCredentials.SecretKey);

        var cognitoUserPoolId = args[3];
        logger.Information("Cognito User Pool Id: {0}", cognitoUserPoolId);

        try
        {
            await ImportUsers(authenticationDatabaseConnection, basicAwsCredentials, logger, cognitoUserPoolId);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error importing users");
            throw;
        }
    }

    private static async Task ImportUsers(SqlConnection authenticationDatabaseConnection,
        BasicAWSCredentials basicAwsCredentials, ILogger logger, string cognitoUserPoolId)
    {
        do
        {
            await Task.Delay(500);

            var usersToImport = await authenticationDatabaseConnection
                .QueryAsync<AuthenticationUser>(SqlCommandConstants.UsersToImportQuery);

            var cognitoClient = new AmazonCognitoIdentityProviderClient(basicAwsCredentials,
                new AmazonCognitoIdentityProviderConfig
                {
                    RegionEndpoint = RegionEndpoint.USEast1
                });

            foreach (var userToImport in usersToImport)
            {
                logger.Information("Import ASFSAUserID: {ASFSAUserID} Email: {Email1}", userToImport.ASFSAUserID,
                    userToImport.Email1);

                var createUserResponse = await cognitoClient.AdminCreateUserAsync(new AdminCreateUserRequest
                {
                    UserPoolId = cognitoUserPoolId,
                    Username = AuthenticationUser.CreateUniqueUserName(),
                    UserAttributes = AuthenticationUser.ToUserAttribute(userToImport)?.ToList(),
                });

                if (createUserResponse.HttpStatusCode != HttpStatusCode.OK)
                {
                    logger.Error("Unable to create user {ASFSAUserID} Email: {Email1} {@createUserResponse}",
                        userToImport.ASFSAUserID, userToImport.Email1, createUserResponse);
                    continue;
                }

                await authenticationDatabaseConnection.ExecuteAsync(SqlCommandConstants.LogCreateUser, new
                {
                    ASFSAUserID = userToImport.ASFSAUserID,
                    UserName = createUserResponse.User.Username,
                    CreatedUser = true,
                    CognitoUserPoolId = cognitoUserPoolId
                });

                var adminSetUserPasswordAsyncResponse =
                    await cognitoClient.AdminSetUserPasswordAsync(new AdminSetUserPasswordRequest
                    {
                        UserPoolId = cognitoUserPoolId,
                        Username = createUserResponse.User.Username,
                        Password = userToImport.Password,
                        Permanent = true
                    });

                if (adminSetUserPasswordAsyncResponse.HttpStatusCode != HttpStatusCode.OK)
                {
                    logger.Error(
                        "Unable to set password on user account {ASFSAUserID} Email: {Email1} {@createUserResponse}",
                        userToImport.ASFSAUserID, userToImport.Email1, createUserResponse);
                    continue;
                }

                await authenticationDatabaseConnection.ExecuteAsync(SqlCommandConstants.UpdateUserWithPasswordApplied, new
                {
                    ASFSAUserID = userToImport.ASFSAUserID,
                });
            }
        } while (await authenticationDatabaseConnection.ExecuteAsync(SqlCommandConstants.UsersLeftToImportQuery) > 0);
    }
}
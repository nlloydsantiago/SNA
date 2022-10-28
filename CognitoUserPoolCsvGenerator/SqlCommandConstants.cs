namespace CognitoUserPoolCsvGenerator
{
    internal class SqlCommandConstants
    {
        public static string UsersToImportQuery = @"
select  top 10 * 
from    ASFSAUser u 
where   ltrim(rtrim(isnull(u.Email1, ''))) <> ''
and not exists (
        select id 
        from AuraASFAUserImportLog l 
        where l.ASFSAUserID = u.ASFSAUserID)";

        public static string UsersLeftToImportQuery = @"
select  count(u.ID) 
from    ASFSAUser u 
where   ltrim(rtrim(isnull(u.Email1, ''))) <> ''
and not exists (
        select id 
        from AuraASFAUserImportLog l 
        where l.ASFSAUserID = u.ASFSAUserID)
";

        public static string LogCreateUser = @"
insert into AuraASFAUserImportLog(ASFSAUserID, UserName, CreatedUser, CognitoUserPoolId) 
values (@ASFSAUserID, @UserName, @CreatedUser, @CognitoUserPoolId);";

        public static string UpdateUserWithPasswordApplied = @"
Update AuraASFAUserImportLog 
set AppliedPassword = 1
Where ASFSAUserID = @ASFSAUserID
";
    }
}

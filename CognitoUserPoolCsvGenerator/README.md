# Create Users from SNA Custom Database

Add a log table to the database so that we can track our progress (if it doesn't already exist):

```sql
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuraASFAUserImportLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ASFSAUserID] [int] NULL,
	[UserName] [nvarchar](255) NULL,
	[CreatedUser] [bit] NULL,
	[AppliedPassword] [bit] NULL,
	[DateCreated] [datetime] NOT NULL,
	[CognitoUserPoolId] [nvarchar](255) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuraASFAUserImportLog] ADD  CONSTRAINT [DF_AuraASFAUserImportLog_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO

```

Invoke the program with either in VS with

/Properties/launchSettings.json
```json
{
  "profiles": {
    "CognitoUserPoolCsvGenerator": {
      "commandName": "Project",
      "commandLineArgs": "\"server=192.168.0.23;database=ASFSA_Authen;Integrated Security=FALSE;user=*** user name ***;pwd=*** password ***;Connect Timeout=180;pooling=true;Max Pool Size=100;\"  \r\*** aws access key ***\r\n*** aws access secret ***\r\*** cognito user pool id ***"
    }
  }
}
```

...or your preferred editor with the understanding that .net is parsing the argument array `args[]` as a space delimted set of parameters.
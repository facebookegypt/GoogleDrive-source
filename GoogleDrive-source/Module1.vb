''''https://adonetaccess2003.blogspot.com
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Drive.v3
Imports Google.Apis.Drive.v3.Data
Imports Google.Apis.Services
Imports Google.Apis.Util.Store
Imports System.IO
Imports System.Threading
'Save "credentials.json" in your project directory (i.e D:\Google-Drive-example\)
'Refresh 'Solution explorer', show all files, include "credentials.json" into your project
'From "Properties panel" change "Copy to Output Directory" to "Copy Always".
Module Module1
    'If modifying these scopes, delete your previously saved credentials
    'at ~/.credentials/drive-dotnet-quickstart.json
    Dim Scopes() As String = {DriveService.Scope.DriveReadonly}
    Dim ApplicationName As String = "Quickstart"
    Private Service As DriveService = New DriveService
    Public Function Main(textbox As TextBox) As String
        Dim credential As UserCredential
        'replace my credentials with yours, I will erase my credentaisl before I commit this to POST.
        Using Stream = New FileStream("credentials.json", FileMode.Open, FileAccess.Read)
            'The file token.json stores the user's access and refresh tokens, and is created
            'automatically when the authorization flow completes for the first time.
            Dim credPath As String = "token.json"
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(Stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    New FileDataStore(credPath, True)).Result
            'Console.WriteLine("Credential file saved to: " + credPath)
            textbox.Text = ("Credential file saved to: " + credPath + vbCrLf)
        End Using
        'Create Drive API service.
        Dim Service = New DriveService(New BaseClientService.Initializer() With
            {
                .HttpClientInitializer = credential,
                .ApplicationName = ApplicationName
            })
        ' Define parameters of request.
        Dim listRequest As FilesResource.ListRequest = Service.Files.List()
        listRequest.PageSize = 10
        listRequest.Fields = "nextPageToken, files(id, name)"
        'List files.
        Dim files As IList(Of Data.File) = listRequest.Execute().Files
        'Console.WriteLine("Files:")
        textbox.Text += "files :" & vbCrLf
        If (files IsNot Nothing And files.Count > 0) Then
            For Each file In files
                'Console.WriteLine("{0} ({1})", file.Name, file.Idf
                textbox.Text += (file.Name & " : " & file.Id) & vbCrLf
            Next
        Else
            'Console.WriteLine("No files found.")
            textbox.Text = ("No files found.")
        End If
        'Console.Read()
        Return textbox.Text
    End Function
End Module
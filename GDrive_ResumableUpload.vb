Imports System.Text
Imports Newtonsoft.Json
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Security.Cryptography
Imports System.Runtime.InteropServices
Imports Google.Apis.Drive.v3
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Services
Imports Google.Apis.Util.Store
Public Class GDrive_ResumableUpload
Public Sub FileUpload(ByVal localfileName As String)
        Dim Thosecredentials = GoogleWebAuthorizationBroker.AuthorizeAsync(New ClientSecrets With {.ClientId = clientID,
                                                                                 .ClientSecret = clientSecret},
                                                            Scopes,
                                                            Environment.UserName,
                                                            Threading.CancellationToken.None,
                                                            New FileDataStore("speec.GoogleDrive.Auth.Store")).Result
        'Create Serive
        Dim Service = New DriveService _
            (New BaseClientService.Initializer() With {
            .HttpClientInitializer = Thosecredentials,
            .ApplicationName = ApplicationName
             })
        Try
            Dim info As System.IO.FileInfo = New System.IO.FileInfo(localfileName)
            Dim fileSize As ULong = CULng(info.Length)
            Dim uploadStream = New System.IO.FileStream(localfileName, System.IO.FileMode.Open, System.IO.FileAccess.Read)
            Dim insert = Service.Files.Create(New Google.Apis.Drive.v3.Data.File With {
                .Name = localfileName,
                .Parents = New List(Of String) From {
                    "root"
                }
            }, uploadStream, "application/octet-stream")
            Dim uploadUri As Uri = insert.InitiateSessionAsync().Result
            Dim chunk_size As Integer = Google.Apis.Upload.ResumableUpload.MinimumChunkSize
            Dim bytesSent As Integer = 0

            While uploadStream.Length <> uploadStream.Position
                Dim temp As Byte() = New Byte(chunk_size - 1) {}
                Dim cnt As Integer = uploadStream.Read(temp, 0, temp.Length)
                If cnt = 0 Then Exit While
                Dim httpRequest As HttpWebRequest = CType(WebRequest.Create(uploadUri), HttpWebRequest)
                httpRequest.Method = "PUT"
                httpRequest.Headers("Authorization") = "Bearer " & (CType(Service.HttpClientInitializer, UserCredential)).Token.AccessToken
                httpRequest.ContentLength = CLng(cnt)
                httpRequest.Headers("Content-Range") = String.Format("bytes {0}-{1}/{2}", bytesSent, bytesSent + cnt - 1, fileSize)

                Using requestStream As System.IO.Stream = httpRequest.GetRequestStreamAsync().Result
                    requestStream.Write(temp, 0, cnt)
                End Using

                Dim httpResponse As HttpWebResponse

                Try
                    httpResponse = CType(httpRequest.GetResponse(), HttpWebResponse)
                Catch ex As WebException
                    httpResponse = CType(ex.Response, HttpWebResponse)
                End Try

                If httpResponse.StatusCode = HttpStatusCode.OK Then
                ElseIf CInt(httpResponse.StatusCode) <> 308 Then
                    Exit While
                End If

                bytesSent += cnt
                Debug.WriteLine("Uploaded " & bytesSent.ToString())
            End While

            If bytesSent <> uploadStream.Length Then
            End If

        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub
    End Class

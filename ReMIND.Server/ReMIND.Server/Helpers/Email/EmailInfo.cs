using System.Text.Json.Serialization;

namespace ReMIND.Server.Helpers.Email
{
    public static class EmailInfo
    {
        public static string HostEmail = "remindapplication@yahoo.com";
        public static string HostPassword = "BuiltOnHpPavilion2019.";

        public static string SmtpClient = "smtp.mail.yahoo.com";
        public static int Port = 465;
        
        public static string footer = "Ralex, Jagodina";
        
        public static string setBody(string title, string name, string contact, string deadline, string description) {
            string email = $@"
            <!doctype html>
            <html>
                <head>
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
                    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
                    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
                    <link href=""https://fonts.googleapis.com/css2?family=Roboto&display=swap"" rel=""stylesheet"">
                    <title>{title}</title>"
                     + System.IO.File.ReadAllText(@"./Helpers/Email/CSS.txt") + 
                    $@"
                </head>
                <body>
                    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""body"">
                    <tr>
                        <td>&nbsp;</td>
                        <td class=""container"">
                        <div class=""content"">
                            <img src=""https://i.imgur.com/gRTfPVm.png"" />
                            <!-- START CENTERED WHITE CONTAINER -->
                            <table role=""presentation"" class=""main"">

                            <!-- START MAIN CONTENT AREA -->
                            <tr>
                                <td class=""wrapper"">
                                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                    <tr>
                                    <td>
                                    <h2>{name}</h2>
                                    <hr/>
                                        <div>
                                        <h3 class=""float-left"">{contact}</h3>
                                        <h3 class=""float-right"">{deadline}</h3>
                                        </div>
                                        
                                    </td>
                                    </tr>
                                    <tr>
                                    <td>
                                    <hr/>
                                        <div class=""description"">
                                        <p>{description}</p>
                                        </div>
                                    </td>
                                    </tr>
                                </table>
                                </td>
                            </tr>

                            <!-- END MAIN CONTENT AREA -->
                            </table>
                            <!-- END CENTERED WHITE CONTAINER -->

                            <!-- START FOOTER -->
                            <div class=""footer"">
                            <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                <td class=""content-block"">
                                    <span class=""apple-link"" style=""font-weight: bold; margin-bottom: 2px"">{footer}</span>
                                    <br><a href=""mailto:rasicdnikola@gmail.com"">Contact ReMIND Support</a>.
                                </td>
                                </tr>
                            </table>
                            </div>
                            <!-- END FOOTER -->

                        </div>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    </table>
                </body>
                </html>
            ";

            return email;
        }             
    }
}
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace JieRuntime.EMail
{
    /// <summary>
    /// 提供基于 SMTP 协议的邮件发送服务
    /// </summary>
    public class SmtpEmail
    {
        #region --字段--
        private readonly SmtpClient smtpClient;
        private readonly MailMessage mailMessage;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="SmtpEmail"/> 类的新实例
        /// </summary>
        public SmtpEmail ()
        {
            this.smtpClient = new SmtpClient ();
            this.mailMessage = new MailMessage ();
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 设置 SMTP 邮件服务器的地址
        /// </summary>
        /// <param name="host">目标 SMTP 服务器地址</param>
        /// <param name="port">目标 SMTP 服务器的端口号, 默认: 25</param>
        /// <param name="enableSsl">启用 SSL 安全连接, 默认: <see langword="false"/></param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="host"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetServer (string host, int port = 25, bool enableSsl = false)
        {
            this.smtpClient.Host = host ?? throw new ArgumentNullException (nameof (host));
            this.smtpClient.Port = port;
            this.smtpClient.EnableSsl = enableSsl;
            return this;
        }

        /// <summary>
        /// 设置邮件发送者
        /// </summary>
        /// <param name="address">发件人邮箱账号</param>
        /// <param name="password">发件人邮箱密码</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> 或 <paramref name="password"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetSender (string address, string password)
        {
            if (address == null)
            {
                throw new ArgumentNullException (nameof (address));
            }

            if (password == null)
            {
                throw new ArgumentNullException (nameof (password));
            }

            this.mailMessage.From = new MailAddress (address);
            this.smtpClient.Credentials = new NetworkCredential (address, password);
            return this;
        }

        /// <summary>
        /// 设置邮件发送者
        /// </summary>
        /// <param name="address">发件人邮箱账号</param>
        /// <param name="password">发件人邮箱密码</param>
        /// <param name="domain">与发件人凭证关联的域</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="address"/>、<paramref name="password"/> 或 <paramref name="domain"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetSender (string address, string password, string domain)
        {
            if (address == null)
            {
                throw new ArgumentNullException (nameof (address));
            }

            if (password == null)
            {
                throw new ArgumentNullException (nameof (password));
            }

            if (domain == null)
            {
                throw new ArgumentNullException (nameof (domain));
            }

            this.mailMessage.From = new MailAddress (address);
            this.smtpClient.Credentials = new NetworkCredential (address, password, domain);
            return this;
        }

        /// <summary>
        /// 设置邮件接收者
        /// </summary>
        /// <param name="address">收件人邮箱账号</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetReceiver (string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException (nameof (address));
            }

            this.mailMessage.To.Add (address);
            return this;
        }

        /// <summary>
        /// 设置邮件抄送
        /// </summary>
        /// <param name="address">抄送邮箱账号</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetCarbonCopy (string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException (nameof (address));
            }

            this.mailMessage.CC.Add (address);
            return this;
        }

        /// <summary>
        /// 设置邮件主题
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subject"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetSubject (string subject)
        {
            this.mailMessage.Subject = subject ?? throw new ArgumentNullException (nameof (subject));
            return this;
        }

        /// <summary>
        /// 设置邮件正文
        /// </summary>
        /// <param name="body">邮件正文</param>
        /// <param name="htmlBody">邮件正文是否使用 HTML 代码</param>
        /// <param name="bodyEncoding">邮件正文的编码</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="body"/> 或 <paramref name="bodyEncoding"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetBody (string body, bool htmlBody, Encoding bodyEncoding)
        {
            this.mailMessage.Body = body ?? throw new ArgumentNullException (nameof (body));
            this.mailMessage.IsBodyHtml = htmlBody;
            this.mailMessage.BodyEncoding = bodyEncoding ?? throw new ArgumentNullException (nameof (bodyEncoding));
            return this;
        }

        /// <summary>
        /// 设置邮件消息, 并指定使用的编码
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="bodyEncoding">邮件正文的编码</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subject"/>、<paramref name="body"/> 或 <paramref name="bodyEncoding"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetMessage (string subject, string body, Encoding bodyEncoding)
        {
            return this.SetMessage (subject, body, false, bodyEncoding);
        }

        /// <summary>
        /// 设置邮件消息, 并采用 <see cref="Encoding.UTF8"/> 编码
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="htmlBody">邮件正文是否使用 HTML 代码</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subject"/> 或 <paramref name="body"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetMessage (string subject, string body, bool htmlBody = true)
        {
            return this.SetMessage (subject, body, htmlBody, Encoding.UTF8);
        }

        /// <summary>
        /// 设置邮件消息
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="htmlBody">邮件正文是否使用 HTML 代码</param>
        /// <param name="bodyEncoding">邮件正文的编码</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subject"/>、<paramref name="body"/> 或 <paramref name="bodyEncoding"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetMessage (string subject, string body, bool htmlBody, Encoding bodyEncoding)
        {
            this.SetSubject (subject);
            this.SetBody (body, htmlBody, bodyEncoding);
            return this;
        }

        /// <summary>
        /// 从文件读取内容位置为邮件正文
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="bodyEncoding">邮件正文的编码</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> 或 <paramref name="bodyEncoding"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetBodyFromFile (string fileName, Encoding bodyEncoding)
        {
            return this.SetBodyFromFile (fileName, true, bodyEncoding);
        }

        /// <summary>
        /// 从文件读取内容位置为邮件正文, 并使用 <see cref="Encoding.UTF8"/> 编码
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="htmlBody">邮件正文是否使用 HTML 代码</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetBodyFromFile (string fileName, bool htmlBody = true)
        {
            return this.SetBodyFromFile (fileName, htmlBody, Encoding.UTF8);
        }

        /// <summary>
        /// 从文件读取内容位置为邮件正文
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="htmlBody">邮件正文是否使用 HTML 代码</param>
        /// <param name="bodyEncoding">邮件正文的编码</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> 或 <paramref name="bodyEncoding"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetBodyFromFile (string fileName, bool htmlBody, Encoding bodyEncoding)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException (nameof (fileName));
            }

            if (bodyEncoding == null)
            {
                throw new ArgumentNullException (nameof (bodyEncoding));
            }

            if (File.Exists (fileName))
            {
                string filePath = Path.GetFullPath (fileName);
                using FileStream fsRead = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                using StreamReader reader = new (fsRead, bodyEncoding);
                string body = reader.ReadToEnd ();
                this.SetBody (body, htmlBody, bodyEncoding);
            }

            return this;
        }

        /// <summary>
        /// 设置邮件附件
        /// </summary>
        /// <param name="fileName">附件路径</param>
        /// <param name="attachmentName">附件文件名</param>
        /// <returns>当前 <see cref="SmtpEmail"/> 实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> 或 <paramref name="attachmentName"/> 为 <see langword="null"/></exception>
        public SmtpEmail SetAttachment (string fileName, string attachmentName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException (nameof (fileName));
            }

            if (attachmentName == null)
            {
                throw new ArgumentNullException (nameof (attachmentName));
            }

            this.mailMessage.Attachments.Add (new Attachment (fileName) { Name = attachmentName });
            return this;
        }

        /// <summary>
        /// 将当前当前实例描述的邮件发送请求投递给 SMTP 服务器
        /// </summary>
        public bool Send ()
        {
            try
            {
                this.smtpClient.Send(this.mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
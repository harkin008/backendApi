using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using System.Net.Http;

namespace test_webApiIdentity2.Infrastructure
{
    public class imageUploadFormatter : FormUrlEncodedMediaTypeFormatter
    {

        private static Type _supportedType = typeof(byte[]);
        private const int BufferSize = 8192;
        public imageUploadFormatter()
            : base()
        {

            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpg"));
            //SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/png"));
        }

        public override bool CanReadType(Type type)
        {

            //throw new NotImplementedException();
            return true;//(type == typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter));
        }

        public override bool CanWriteType(Type type)
        {
            //throw new NotImplementedException();
            return true;
        }


        //protected  Task<object> OnReadFromSteramAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, StreamingContext formatterContext)
        //{
        //    var context = HttpContext.Current.Request.Form;//.ReadAsMultipartAsync().Result;


        //    return Task.Factory.StartNew<object>(() =>
        //    {
        //        //byte[] filebyte = new byte[stream.Length];
        //        //stream.Read(filebyte, 0, (int)filebyte.Length);

        //        return new MultiFormKeyValueModel(contents) ;
        //    });
        //}
        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var parts = await content.ReadAsMultipartAsync();
        var obj = Activator.CreateInstance(type);
        var propertiesFromObj = obj.GetType().GetProperties().ToList();//  GetRuntimeProperties().ToList();
 
        foreach (var property in propertiesFromObj.Where(x => x.PropertyType == typeof(ContentModel)))
        {
            var file = parts.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Contains(property.Name));
 
            if (file == null || file.Headers.ContentLength > 0 || property.GetType() != typeof(ContentModel))
        {
            var formData =
                parts.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Contains(property.Name));
 
            if (formData == null) continue;
 
            try
            {
                var strValue = formData.ReadAsStringAsync().Result;
                var valueType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var value = Convert.ChangeType(strValue, valueType);
                property.SetValue(obj, value);
            }
            catch (Exception e)
            {
            }
        }

          
        //return Task.Factory.StartNew<object> (() =>{
        
       
        }
        return Task.FromResult<object>(obj);
        }
        private byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0 )
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        protected virtual Task<object> ReadFromStreamAsync1(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskSource = new TaskCompletionSource<object>();
            try
            {
                var ms = new MemoryStream();
                stream.CopyTo(ms, BufferSize);
                taskSource.SetResult(ms.ToArray());
            }
            catch (Exception e)
            {
                taskSource.SetException(e);
            }
            return taskSource.Task;
        }

        protected Task<object> OnWriteToSteramAsync(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            throw new NotImplementedException();

        }
    }
    public class ContentModel
    {
        public ContentModel(string filename, int contentLength, byte[] content)
        {
            Filename = filename;
            ContentLength = contentLength;
            Content = content;
        }

        public string Filename { get; set; }

        public int ContentLength { get; set; }

        public byte[] Content { get; set; }

    }
    //public class MultiFormKeyValueMode 
    //{
    //    NameValueCollection _collection;
    //    public MultiFormKeyValueMode(NameValueCollection collection)
    //    {

    //    }

    //}
}
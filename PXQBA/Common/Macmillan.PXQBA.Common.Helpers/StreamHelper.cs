using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class StreamHelper
    {

        /// <summary>
        /// Reads byte array from stream
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Serializes object to byte array
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize</param>
        /// <returns>Serialized byte array</returns>
        public static byte[] SerializeToByte(object objectToSerialize)
        {
            if (objectToSerialize == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, objectToSerialize);
            return ms.ToArray();
        }

        /// <summary>
        /// Deserialize byte array to object
        /// </summary>
        /// <param name="arrBytes">Array to deserialize</param>
        /// <returns></returns>
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return binForm.Deserialize(memStream);
        }
    }
}

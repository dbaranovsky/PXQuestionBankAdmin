using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Macmillan.PXQBA.Common.Helpers.Tests
{
    [TestClass]
    public class StreamHelperTest
    {
        [Serializable]
        class Dummy
        {
            public string Text { get; set; }
        }

        [TestMethod]
        public void SerializeToByte_DummyClass_CorrectByteArray()
        {
            Dummy dummy = new Dummy()
                          {
                              Text = "Dummy"
                          };

            var array = StreamHelper.SerializeToByte(dummy);

            Dummy dummy2 = (Dummy)StreamHelper.ByteArrayToObject(array);

            Assert.IsTrue(dummy.Text == dummy2.Text);
        } 


        [TestMethod]
        public void ReadFully_Stream_CorrectByteArray()
        {
            byte[] bytes = new[] { byte.MinValue, byte.MaxValue};
            Stream stream =new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0L;
            var result = StreamHelper.ReadFully(stream);

            Assert.IsTrue(result[0] == bytes[0]);
            Assert.IsTrue(result[1] == bytes[1]);
            Assert.IsTrue(result.Length == bytes.Length);
        }
    }
}

using System;
using System.Collections.Generic;
using Apollo.AIM.SNAP.Model;
using NUnit.Framework;

namespace Apollo.AIM.SNAP.Test
{

    [TestFixture]
    public class Utilities_Test
    {

        [Test]
        public void ShouldReturnJson()
        {
            var responses = new List<WebMethodResponse>
                            {
                                new WebMethodResponse() {Message = "Hello", Success = true, Title = "World"},
                                new WebMethodResponse() {Message = "Speaking", Success = false, Title = "To Me!"},
                            };

            var jstring = responses.ToJSONString();
            Console.WriteLine("Json String: " + jstring);


            var responses2 = jstring.FromJSONStringToObj<List<WebMethodResponse>>();
            Assert.IsInstanceOfType(typeof(List<WebMethodResponse>), responses2);
            responses2.ForEach(r => Console.WriteLine(r.Message));
            Assert.IsInstanceOfType(typeof(WebMethodResponse), responses2[0]);
            Assert.IsTrue(responses2[0].Success);
            Assert.IsInstanceOfType(typeof(WebMethodResponse), responses2[1]);
            Assert.IsFalse(responses2[1].Success);
            Console.WriteLine(responses2[1].Message + ", " + responses2[1].Success + ", " + responses2[1].Title);
        }
    }
}

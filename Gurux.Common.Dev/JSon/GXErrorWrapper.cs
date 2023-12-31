﻿using System;
using System.IO;

/* Необъединенное слияние из проекта "Gurux.Common (netcoreapp3.1)"
До:
using System.Linq;
using System.Text;
После:
using System.IO;
using System.Linq;
*/

/* Необъединенное слияние из проекта "Gurux.Common (netstandard2.1)"
До:
using System.Linq;
using System.Text;
После:
using System.IO;
using System.Linq;
*/
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
/* Необъединенное слияние из проекта "Gurux.Common (netcoreapp3.1)"
До:
using System.IO;
using System.Runtime.Serialization;
После:
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
*/

/* Необъединенное слияние из проекта "Gurux.Common (netstandard2.1)"
До:
using System.IO;
using System.Runtime.Serialization;
После:
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
*/


namespace Gurux.Common.JSon
{
    /// <summary>
    /// GXErrorWrapper is used to save occurred excetion so it can be move over HttpWebRequest.
    /// </summary>
    [DataContractAttribute]
    public class GXErrorWrapper
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXErrorWrapper()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ex">Occurred exception.</param>
        public GXErrorWrapper(Exception ex)
        {
            SetException(ex);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">serialized data.</param>
        public GXErrorWrapper(byte[] data)
        {
            Data = data;
        }

        /// <summary>
        /// Get occurred exception.
        /// </summary>
        /// <returns></returns>
        public Exception GetException()
        {
            if (Data == null)
            {
                return null;
            }
            Exception result;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(Data))
            {
                result = (Exception)bf.Deserialize(stream);
                stream.Close();
            }
            return result;
        }

        /// <summary>
        /// Set occurred exception.
        /// </summary>
        /// <param name="ex"></param>
        private void SetException(Exception ex)
        {
            Message = ex.Message;
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, ex);
                Data = stream.ToArray();
                stream.Close();
            }
        }

        /// <summary>
        /// Occurred exception.
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Exception data that is moved over HttpWebRequest.
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte[] Data
        {
            get;
            private set;
        }
    };
}

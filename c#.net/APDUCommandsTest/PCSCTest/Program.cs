using System;
using System.Collections.Generic;
using System.Text;

namespace PCSCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            long retCode;
            int hContext = 1;
            int ReaderCount = 0;
            int Protocol = 0;
            int hCard = 0;
            string defaultReader = null;
            int SendLen, RecvLen;

            byte[] SendBuff = new byte[262];
            byte[] RecvBuff = new byte[262];

            ModWinsCard.SCARD_IO_REQUEST ioRequest;

            retCode = ModWinsCard.SCardEstablishContext(ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);
            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                System.Diagnostics.Debug.WriteLine(ModWinsCard.GetScardErrMsg(retCode));
            }

            retCode = ModWinsCard.SCardListReaders(hContext, null, null, ref ReaderCount);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                System.Diagnostics.Debug.WriteLine(ModWinsCard.GetScardErrMsg(retCode));
            }

            byte[] retData = new byte[ReaderCount];
            byte[] sReaderGroup = new byte[0];

            //Get the list of reader present again but this time add sReaderGroup, retData as 2rd & 3rd parameter respectively.
            retCode = ModWinsCard.SCardListReaders(hContext, sReaderGroup, retData, ref ReaderCount);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                System.Diagnostics.Debug.WriteLine(ModWinsCard.GetScardErrMsg(retCode));
            }

            //Convert retData(Hexadecimal) value to String 
            string readerStr = System.Text.ASCIIEncoding.ASCII.GetString(retData);
            string[] rList = readerStr.Split('\0');

            foreach (string readerName in rList)
            {
                if (readerName != null && readerName.Length > 1)
                {
                    defaultReader = readerName;
                    break;
                }
            }

            if (defaultReader != null)
            {
                retCode = ModWinsCard.SCardConnect(
                    hContext, 
                    defaultReader, 
                    ModWinsCard.SCARD_SHARE_DIRECT,
                    ModWinsCard.SCARD_PROTOCOL_UNDEFINED, 
                    ref hCard, 
                    ref Protocol);
                //Check if it connects successfully
                if (retCode != ModWinsCard.SCARD_S_SUCCESS)
                {
                    string error = ModWinsCard.GetScardErrMsg(retCode);
                }
                else
                {
                    int pcchReaderLen = 256;
                    int state = 0;
                    byte atr = 0;
                    int atrLen = 255;

                    //get card status
                    retCode = ModWinsCard.SCardStatus(hCard, defaultReader, ref pcchReaderLen, ref state, ref Protocol, ref atr, ref atrLen);

                    if (retCode != ModWinsCard.SCARD_S_SUCCESS)
                    {
                        return;
                    }


                }
            }
        }
    }
}

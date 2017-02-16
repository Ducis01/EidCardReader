/* ****************************************************************************

    CardCard class to get labels and files from the ID Card with by value 
    references.
    Author: Nicolas Van Wallendael <nicolasvanwallendael@icloud.com>
    Copyright (C) 2017, Nicolas Van Wallendael

    Inspired and using :: http://github.com/Fedict/eid-mw

**************************************************************************** */

using System;

using Net.Sf.Pkcs11;
using Net.Sf.Pkcs11.Objects;
using Net.Sf.Pkcs11.Wrapper;

namespace EidSamples
{
    class CardData
    {
        private Module m = null;
        private String mFileName;

        public CardData()
        {
            mFileName = "beidpkcs11.dll";
        }
        public CardData(String moduleFileName)
        {
            mFileName = moduleFileName;
        }

        /// <summary>
        /// Generic function to get string data objects from labels and files
        /// </summary>
        /// <param name="labels">Values of label attribute of the object</param>
        /// <param name="files"> Values of file attribute of the object</param>
        /// <param name="outL">  Out value for the labels</param>
        /// <param name="outF">  Out value for the files</param>
        /// <returns></returns>
        public void GetData(String[] labels, String[] files, String[] outL, byte[][] outF)
        {

            if (m == null)
            {
                m = Module.GetInstance(mFileName);
            }
            // pkcs11 module init
            //m.Initialize();
            try
            {
                // Get the first slot (cardreader) with a token
                Slot[] slotlist = m.GetSlotList(true);
                if (slotlist.Length > 0)
                {
                    Slot slot = slotlist[0];

                    Session session = slot.Token.OpenSession(true);

                    // Search for objects
                    // First, define a search template 

                    // "The label attribute of the objects should equal ..."

                    ByteArrayAttribute classAttribute = new ByteArrayAttribute(CKA.CLASS);
                    classAttribute.Value = BitConverter.GetBytes((uint)Net.Sf.Pkcs11.Wrapper.CKO.DATA);

                    ByteArrayAttribute labelAttribute = new ByteArrayAttribute(CKA.LABEL);

                    Data data;
                    int counter, i = 0;
                    P11Object[] foundObjects;

                    // Get all labels
                    if (labels == null) labels = new String[] { };
                    foreach ( String lab in labels) {

                        Console.WriteLine("Getting >>   " + lab);

                        labelAttribute.Value = System.Text.Encoding.UTF8.GetBytes(lab);

                        session.FindObjectsInit(new P11Attribute[] { classAttribute, labelAttribute });

                        foundObjects = session.FindObjects(50);
                        counter = foundObjects.Length;

                        while (counter > 0)
                        {
                            //foundObjects[counter-1].ReadAttributes(session);
                            //public static BooleanAttribute ReadAttribute(Session session, uint hObj, BooleanAttribute attr)
                            data = foundObjects[counter - 1] as Data;
                            /*String label = data.Label.ToString();
                            if (label != null)
                                Console.WriteLine(label); */
                            if (data.Value.Value != null)
                            {
                                outL[i] = System.Text.Encoding.UTF8.GetString(data.Value.Value);
                                Console.WriteLine("\t" + outL[i]);
                            }
                            counter--;
                        }
                        i++;
                        session.FindObjectsFinal();
                    }


                    // Get all files asked as once
                    if (files == null) files = new String[] { };

                    i = 0;
                    foreach (String file in files)
                    {

                        Console.WriteLine("Getting FILE >>   " + file);

                        labelAttribute.Value = System.Text.Encoding.UTF8.GetBytes(file);

                        session.FindObjectsInit(new P11Attribute[] { classAttribute, labelAttribute });

                        foundObjects = session.FindObjects(1);
                        if (foundObjects.Length != 0)
                        {
                            data    = foundObjects[0] as Data;
                            outF[i] = data.Value.Value;
                        }

                        i++;
                        session.FindObjectsFinal();
                    }

                    session.Dispose();
                }
                else
                {
                    Console.WriteLine("No card found\n");
                }
            }
            finally
            {

                // pkcs11 finalize
                m.Dispose();//m.Finalize_();
                m = null;
            }
            return ;
        }

    }
}

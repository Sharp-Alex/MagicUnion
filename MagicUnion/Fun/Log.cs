using MagicUnion.Fun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CreateRecord.Fun
{
    public static class Log
    {
        #region private 

        private static readonly LogDestructor Finalise = new LogDestructor();

        private static StreamWriter _stream = null;

        private static object _mutex = new object();

        private static void StreamClose()
        {
            try
            {                
                _stream?.Close();
                _stream = null;
            }
            catch(Exception e)
            {
            }
        }

        sealed class LogDestructor
        {
            ~LogDestructor()
            {
                lock (_mutex)
                {
                    StreamClose();
                }
            }

        }
        #endregion

        public static void Close()
        {
            lock (_mutex)
            {
                StreamClose();
            }
        }
        public static bool IsOpen()
        {
            lock(_mutex)
            {
                return _stream != null;
            }
        }

        public static Result<Empty> OpenFileIfNot(string fullfilename, FileMode mode = FileMode.OpenOrCreate)
        {
            lock (_mutex)
            {
                if (_stream != null)
                    return Empty.One;

                var r = Try.Action(() =>
                {                    
                    _stream = new StreamWriter(File.Open(fullfilename, mode), Encoding.UTF8);
                });

                return r;
            }
        }
        public static Result<Empty> OpenFile(string fullfilename, FileMode mode = FileMode.OpenOrCreate)
        {
            lock (_mutex)
            {
                var r = Try.Action(() =>
                {
                    StreamClose();
                    _stream = new StreamWriter(File.Open(fullfilename, mode), Encoding.UTF8);
                });

                return r;
            }
        }

        public static string Now()
        {
            lock (_mutex)
            {
                return DateTime.Now.ToString("MM.dd HH:mm:ss.ffffff");
            }
        }

        public static string NowFileName()
        {
            lock (_mutex)
            {
                return DateTime.Now.ToString("MM.dd_HH.mm.ss.ffffff");
            }
        }

        public static Empty Write(string text)
        {
            lock (_mutex)
            {
                if(_stream!=null)
                {
                    _stream.WriteLine(Now() + "~>" + text);
                    _stream.Flush();

                }
                
                return Empty.One;
            }
        }
        public static Result<T> LogWrite<T>(this Result<T> result, string text)
        {
            Write(text);
            return result;
        }
    }
}

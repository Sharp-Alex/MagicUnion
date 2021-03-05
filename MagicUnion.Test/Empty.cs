namespace MagicUnion.Test
{
    public partial class CreateRecordTest
    {
        /// <summary>
        /// Empty is equivalent of void in functional programming.
        /// </summary>
        public class Empty
        {
            private Empty()
            { 
            }

            public static Empty None = new Empty();
        }

    }
}

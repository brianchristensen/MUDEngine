using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUDInterface.Messaging;

namespace MUDInterface.Entities
{
    public abstract class GameEntity : IDisposable
    {
        private Guid _id;
        public Guid ID { get { return _id; } }

        public int Location { get; set; }
        public string Name { get; set; }        

        public GameEntity(int _location, string _name = null)
        {
            _id = Guid.NewGuid();            

            Location = _location;

            if (_name == null)
                Name = "Unnamed";
            else
                Name = _name;
        }

        public bool IsDisposed { get; private set; }
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    //perform cleanup here
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GameEntity()
        {
            Dispose(false);
        }

        public abstract void Update();
        public abstract bool HandleMessage(ref Telegram msg);
        public abstract void Display(string connID);
    }
}

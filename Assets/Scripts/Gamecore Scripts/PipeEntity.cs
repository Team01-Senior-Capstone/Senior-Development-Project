using System.IO;
using System.Collections;

namespace Gamecore {

    class PipeEntity {

        private int height;
        private PiranhaPlant piranhaPlant = null;

        public PipeEntity () {
            
            this.height = 0;
        }

        public int getHeight () {
            return this.height;
        }

        public bool isCompleted () {
            
            return height == 3 && piranhaPlant != null;
        }
    }
}
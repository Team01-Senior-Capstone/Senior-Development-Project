using System;

namespace Gamecore {

    [Serializable]
    class PipeEntity {

        private int height;
        private PiranhaPlant piranhaPlant = null;

        public PipeEntity () {
            
            this.height = 1;
        }

        public int getHeight () {
            return this.height;
        }

        public bool isCompleted () {
            
            return height == 4 && piranhaPlant != null;
        }

        public void decreaseHeight()
        {
            if(height == 4)
            {
                piranhaPlant = null;
            }
            this.height -= 1;
        }

        public void increaseHeight () {

            if (height == 1 || height == 2)
                height++;
            else {
                piranhaPlant = new PiranhaPlant();
                height++;
            }

        }
    }
}
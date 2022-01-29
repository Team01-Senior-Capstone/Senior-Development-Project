
namespace Gamecore {

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
            
            return height == 3 && piranhaPlant != null;
        }

        public void increaseHeight () {

            if (height == 1 || height == 2)
                height++;
            else
                piranhaPlant = new PiranhaPlant();
        }
    }
}
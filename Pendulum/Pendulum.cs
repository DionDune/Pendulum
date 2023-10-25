using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pendulum
{
    internal class Pendulum
    {
        public List<Pendulum> Children { get; set; }

        public float pendulumLength { get; set; }
        public float angle { get; set; }
        public float angle_Velocity { get; set; }
        public float angle_Acceleration { get; set; }

        public float CentrePoint_X { get; set; }
        public float CentrePoint_Y { get; set; }

        public float x { get; set; }
        public float y { get; set; }

        public Pendulum()
        {
            pendulumLength = 300;

            angle = ((float)Math.PI / 180F) * 181F;
            angle_Velocity = 0;
            angle_Acceleration = 0.001F;

            CentrePoint_X = 100;
            CentrePoint_Y = 100;

            Children = new List<Pendulum>();
        }

        public void Update(float GravityForce)
        {
            float force = GravityForce * (float)Math.Sin(angle);

            angle_Acceleration = (-1F * force) / pendulumLength;

            angle_Velocity += angle_Acceleration;
            angle += angle_Velocity;
            angle_Velocity *= 0.995F;

            x = (pendulumLength * (float)Math.Sin(angle)) + (float)CentrePoint_X;
            y = (pendulumLength * (float)Math.Cos(angle)) + (float)CentrePoint_Y;

            foreach (Pendulum pendulum in Children)
            {
                pendulum.CentrePoint_X = x;
                pendulum.CentrePoint_Y = y;

                //Runs double over, faster the deeper the recursion
                //pendulum.Update(1);
            }
        }
    }
}

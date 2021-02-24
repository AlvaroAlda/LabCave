using UnityEngine;

namespace KartGame.KartSystems {

    public class TouchInput : BaseInput
    {
        private float horizontalValue = 0;

        public float sensibility = 0.1f;

        public override Vector2 GenerateInput() 
        {
            //Lee entradas de touch
            if (Input.touchCount > 0)
            {
                //Asigna el movimiento delta del touch en la posicion x
                horizontalValue = Input.touches[0].deltaPosition.x * sensibility;
            }

            //Si no hay entradas se mantiene a 0
            else
                horizontalValue = 0;

            return new Vector2 
            {
                x = horizontalValue,

                // Velocidad hacia delante al maximo por defecto
                y = 255
            };
        }

        
    }
}

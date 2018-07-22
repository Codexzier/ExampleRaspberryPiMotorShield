using System;
using Windows.Devices.Gpio;

namespace ExampleRaspberryPiMotorShield
{
    internal class MotorShield
    {
        private const int _outputM1_A = 4;
        private const int _outputM1_B = 8;
        private const int _outputM2_A = 2;
        private const int _outputM2_B = 16;
        private const int _outputM3_A = 32;
        private const int _outputM3_B = 128;
        private const int _outputM4_A = 1;
        private const int _outputM4_B = 64;

        /// <summary>
        /// Pin connected to ST_CP of 74HC595
        /// </summary>
        private GpioPin _latch;

        /// <summary>
        /// Pin connected to SH_CP of 74HC595
        /// </summary>
        private GpioPin _clock;

        /// <summary>
        /// Pin connected to DS of 74HC595
        /// </summary>
        private GpioPin _data;

        /// <summary>
        /// enable the Shiftregister outputs
        /// </summary>
        private GpioPin _regEn;

        private GpioPin _outputM1;
        private GpioPin _outputM2;
        private GpioPin _outputM3;
        private GpioPin _outputM4;

        internal void Init()
        {
            GpioController controller = this.CheckGpioController();

            // raspberryPi Gpio 9 (Pin 21, on Pi Shield MISO) to arduino D12 
            this._latch = controller.OpenPin(9); // oder Gpio 9
            this._latch.SetDriveMode(GpioPinDriveMode.Output);
            this._latch.Write(GpioPinValue.Low);

            // raspberryPi Gpio 27 (Pin 13, on Pi Shield P2) to arduino D4
            this._clock = controller.OpenPin(27);
            this._clock.SetDriveMode(GpioPinDriveMode.Output);
            this._clock.Write(GpioPinValue.Low);

            // raspberryPi Gpio 25 (Pin 22, on Pi Shield P6) to arduino pin D8
            this._data = controller.OpenPin(25); 
            this._data.SetDriveMode(GpioPinDriveMode.Output);
            this._data.Write(GpioPinValue.Low);

            // raspberryPi Gpio 24 (Pin 18, on Pi Shield P5) to arduino pin D7
            this._regEn = controller.OpenPin(24); 
            this._regEn.SetDriveMode(GpioPinDriveMode.Output);
            this._regEn.Write(GpioPinValue.Low);

            // raspberryPi Gpio 10 (Pin 19, on Pi Shield MOSI) to arduino pin D11
            this._outputM1 = controller.OpenPin(10);
            this._outputM1.SetDriveMode(GpioPinDriveMode.Output);
            this._outputM1.Write(GpioPinValue.Low);

            // raspberryPi Gpio 18 (Pin 12, on Pi Shield P1) to arduino pin D3
            this._outputM2 = controller.OpenPin(18);
            this._outputM2.SetDriveMode(GpioPinDriveMode.Output);
            this._outputM2.Write(GpioPinValue.Low);

            // raspberryPi Gpio 23 (Pin 16, on Pi Shield P4) to arduino pin D6
            this._outputM3 = controller.OpenPin(23);
            this._outputM3.SetDriveMode(GpioPinDriveMode.Output);
            this._outputM3.Write(GpioPinValue.Low);

            // raspberryPi Gpio 22 (Pin 15, on Pi Shield P3) to arduino pin D5
            this._outputM4 = controller.OpenPin(22);
            this._outputM4.SetDriveMode(GpioPinDriveMode.Output);
            this._outputM4.Write(GpioPinValue.Low);
        }
        
        /// <summary>
        /// Check gpiocontroller exist.
        /// </summary>
        private GpioController CheckGpioController()
        {
            var controller = GpioController.GetDefault();

            if(controller == null)
            {
                throw new NullReferenceException("no gpio controller");
            }

            return controller;
        }

        /// <summary>
        /// Enable or disable the motorshield
        /// </summary>
        /// <param name="setOn">Take the motor shield.</param>
        public void SetMotorShield(bool setOn) => this._regEn.Write(this.MapBoolToGpioPinValue(!setOn));

        /// <summary>
        /// set all pwm to 0 and disable the motorshield
        /// </summary>
        public void MotorStop()
        {
            this.SetOutputValue(false);
            this.SetMotorShield(false);
        }

        /// <summary>
        /// Motor drives forward. 
        /// Set get value and shift all register to one value and set the motor outputs.
        /// </summary>
        public void SetForeward()
        {
            int motorsOn = _outputM1_B | _outputM2_B | _outputM3_A | _outputM4_A;
            this.SetRunMotors(motorsOn);
            this.SetMotorShield(true);
        }

        /// <summary>
        /// Motor drives backwards. 
        /// Set get value and shift all register to one value and set the motor outputs
        /// </summary>
        public void SetBackward()
        {
            int motorsOn = _outputM1_A | _outputM2_A | _outputM3_B | _outputM4_B;
            this.SetRunMotors(motorsOn);
            this.SetMotorShield(true);
        }

        /// <summary>
        /// Set left motors forward and right motors backwards
        /// </summary>
        public void SetTurnLeft()
        {
            int motorsOn = _outputM1_B | _outputM2_B | _outputM3_B | _outputM4_B;
            this.SetRunMotors(motorsOn);
            this.SetMotorShield(true);
        }

        /// <summary>
        /// Set left motors backwards and right motors forwards.
        /// </summary>
        public void SetTurnRight()
        {
            int motorsOn = _outputM1_A | _outputM2_A | _outputM3_A | _outputM4_A;
            this.SetRunMotors(motorsOn);
            this.SetMotorShield(true);
        }

        /// <summary>
        /// Set target output and level output on motor driver
        /// </summary>
        /// <param name="motorsOn">shift register value to take on the outputs</param>
        private void SetRunMotors(int motorsOn)
        {
            this.SetOutputValue(true);
            this.RegisterWrite(motorsOn);
        }

        /// <summary>
        /// Set level output on motor driver
        /// </summary>
        private void SetOutputValue(bool setOn)
        {
            this._outputM1.Write(this.MapBoolToGpioPinValue(setOn)); 
            this._outputM2.Write(this.MapBoolToGpioPinValue(setOn)); 
            this._outputM3.Write(this.MapBoolToGpioPinValue(setOn)); 
            this._outputM4.Write(this.MapBoolToGpioPinValue(setOn));
        }

        /// <summary>
        /// Map the bool value to the enum GpioPinValue.
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        private GpioPinValue MapBoolToGpioPinValue(bool on) => on ? GpioPinValue.High : GpioPinValue.Low;

        /// <summary>
        /// Write the value to shiftregister to take on the target outputs.
        /// </summary>
        /// <param name="motors">Set value to setup outputs</param>
        private void RegisterWrite(int motors)
        {
            this._latch.Write(GpioPinValue.Low);

            for (int bit = 0; bit < 8; bit++)
            {
                int target = motors & 0x80;
                motors = motors << 1;

                this._data.Write(target == 128 ? GpioPinValue.High : GpioPinValue.Low);

                this._clock.Write(GpioPinValue.High);
                this.SleepWhile(5);
                this._clock.Write(GpioPinValue.Low);
            }

            this._latch.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Dient als ersatz zu Thread.Sleep
        /// </summary>
        /// <param name="count"></param>
        private void SleepWhile(int count)
        {
            for (int i = 0; i < count; i++) { }
        }
    }
}

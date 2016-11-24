using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using WiringPi;

namespace ChristmasServer {
    class Gpio {
        const int ON = 0;
        const int OFF = 1;
        Logger log;
        public gpio_pin[] pins;
        
        /// <summary>
        /// Creates a GPIO object and initalizes the board/pins based on the configuration file
        /// </summary>
        /// <param name="configFileLoc">Location of the configuration file to build the object</param>
        /// TODO: convert these loops into a single loop after assuring that no errors will pop up
        public Gpio(ConfigFile config, Logger log) {
            this.log = log;
            //need a way to easily retrieve/update pinInfo
            pins = new gpio_pin[config.pin_count];
            for (int i = 0; i < config.pin_count; i++){
                Pin tmpPin = config.pins[i];
                pins[i] = new gpio_pin(tmpPin.status, tmpPin.gpio_pin, tmpPin.branch, i);
            }
            //Initialize board itself
            for (int i = 0; i < pins.Length; i++) {
                GPIO.pinMode(pins[i].gpioPin, (int)GPIO.GPIOpinmode.Output);
                GPIO.digitalWrite(pins[i].gpioPin, pins[i].status);
                log.logOK("Pin: {0} or Branch: {1} was set to {2}", pins[i].gpioPin, pins[i].branch, pins[i].status);
            }
        }
        /// <summary>
        /// Turns on the specified pin regardless of whether it's currently on
        /// </summary>
        /// <param name="listAddress" cref="pins">The address of the pin in gpio::pins</param>
        /// <seealso cref="turnOff(int)"/>
        /// <notes>Will silently fail if an invalid parameter is passed</notes>
        public void turnOn (int listAddress) {
            try {
                //Grab a reference first to check if that pin even exists in the list
                //Would rather it fail here than during WiringPi operations
                gpio_pin refPin = pins[listAddress];
                GPIO.digitalWrite(refPin.gpioPin, ON);
                pins[listAddress].status = ON;
            }
            catch (Exception e) {
                log.logError("Failed to turnOn pin at {0}", listAddress);
            }
        }
        /// <summary>
        /// Turns off the specifed pin regardless of whether it's currently on
        /// </summary>
        /// <param name="listAddress" cref="pins">The address of the pin in gpio::pins</param>
        /// <seealso cref="turnOn(int)"/>
        /// <notes>Will Silently fail if an invalid parameter is passed</notes>
        public void turnOff (int listAddress) {
            try {
                //same idea as gpio::turnOn
                gpio_pin refPin = pins[listAddress];
                GPIO.digitalWrite(refPin.gpioPin, OFF);
                pins[listAddress].status = OFF;
            }
            catch (Exception e) {
                log.logError("Failed to turnOff pin at {0}", listAddress);
            }
        }
        /// <summary>
        /// Turns on all the pins in gpio::pins
        /// </summary>
        /// <seealso cref="AllOff"/>
        public void AllOn() {
            for (int i = 0; i < pins.Length; i++) {
                turnOff(pins[i].listAddress);
            }
            log.logOK("Turned on all pins");
        }
        /// <summary>
        /// Turns off all the pins in gpio::pins
        /// </summary>
        /// <seealso cref="AllOn"/>
        public void AllOff() {
            for (int i = 0; i < pins.Length; i++) {
                turnOn(pins[i].listAddress);
            }
            log.logOK("Turned off all pins");
        }
        /// <summary>
        /// Flips the status of a pin. If it's currently off, togglePin will turn it on.
        /// </summary>
        /// <param name="listAddress" cref="pins">The address of the pin in gpio::pins</param>
        public void togglePin (int listAddress) {
            try {
                //same idea as gpio::turnOn
                gpio_pin refPin = pins[listAddress];
                if (refPin.status == ON) {
                    GPIO.digitalWrite(refPin.gpioPin, OFF);
                    pins[listAddress].status = OFF;
                }
                else {
                    GPIO.digitalWrite(refPin.gpioPin, ON);
                    pins[listAddress].status = ON;
                }
            }
            catch (Exception e) {
                log.logError("Failed to togglePin at {0}", listAddress);
            }
        }
    }
    
}

#!/usr/bin/env bash

# Constants
dotnet_install_location="/opt/dotnet/"
dotnet_regex="\/opt\/dotnet\/"
christmaspi_install_location="/opt/ChristmasPi"

function prompt_yes_no {
    while true; do
        read -p "$1 (y/n)" yn
        case $yn in
            [Yy]* ) return 0;;
            [Nn]* ) return 1;;
            * ) echo "Please answer yes or no.";;
        esac
    done
}
function check_root {
    if [ "$EUID" -ne 0 ]; then 
        echo "This install script must be run with root permissions."
        exit
    fi
}
function is_dotnet_installed {
    command -v $dotnet_install_location/dotnet &> /dev/null
    return $?
}
function isServiceInstalled {
    grep=$(systemctl list-units --full -all | grep "$1")
    if [ $? -eq 0 ]; then
        return 0
    else
        return 1
    fi
}
function uninstallService {
    if isServiceInstalled "$1"; then
        echo "Uninstalling $1.service"
        systemctl stop $1.service
        find /etc/systemd/system/ -name $1.service -exec rm -rf {} \;
        systemctl disable $1.service
        systemctl daemon-reload
    fi
}
function main {
    check_root
    uninstallService "ChristmasPi"
    uninstallService "Scheduler"
    rm -rf $christmaspi_install_location
    is_dotnet_installed
    if [ $? -eq 0 ]; then
        prompt_yes_no "Do you want to uninstall dotnet?"
        if [ $? -eq 0 ]; then
            rm -rf $dotnet_install_location
            perl -pi.bak -e "s/export PATH=$dotnet_regex\:\$PATH//g" /home/pi/.bashrc
            perl -pi.bak -e "s/export DOTNET_ROOT=$dotnet_regex//g" /home/pi/.bashrc
    fi
}

main
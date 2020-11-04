#!/usr/bin/env bash

# Constants
dotnet_installer="https://dot.net/v1/dotnet-install.sh"
dotnet_version="3.1"
dotnet_install_location="/opt/dotnet/"
christmaspi_install_location="/usr/local/ChristmasPi"
christmaspi_release_url="https://github.com/Changer098/ChristmasPi/releases/latest/download/ChristmasPi.zip"

# Arguments

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

function is_dotnet_installed {
    command -v dotnet &> /dev/null
    return $?
}

function check_root {
    if [ "$EUID" -ne 0 ]; then 
        echo "This install script must be run with root permissions."
        exit
    fi
}

function install_dotnet {
    cd /tmp/
    if [ -f "dotnet-install.sh" ]; then
        rm -r "dotnet-install.sh"
    fi
    wget $dotnet_installer
    chmod +x dotnet-install.sh
    ./dotnet-install.sh -c $dotnet_version --install-dir $dotnet_install_location
    path_out=$(echo export PATH="$dotnet_install_location:\$PATH")
    root_out=$(echo export DOTNET_ROOT="$dotnet_install_location")
    echo $path_out >> /home/pi/.bashrc
    echo $root_out >> /home/pi/.bashrc
    echo $path_out >> /etc/bash.bashrc
    echo $root_out >> /etc/bash.bashrc
    source /home/$USER/.bashrc
}

function download_christmaspi {
    echo "Downloading latest ChristmasPi from GitHub"
    curl -L $christmaspi_release_url --output ChristmasPi.zip 
    echo "Installing ChristmasPi"
    unzip ChristmasPi.zip -d $christmaspi_install_location
    rm -f ChristmasPi.zip
}

function run_christmaspi {
    echo "Starting Server"
    cd $christmaspi_install_location
    dotnet ChristmasPi.Server.dll
}

function main() {
    check_root
    is_dotnet_installed
    if [ $? -eq 1 ]; then
        install_dotnet
    fi
    if [ -d $christmaspi_install_location ]; then
        echo "ChristmasPi directory already exists, proceeding with this installation will remove that directory."
        prompt_yes_no "Do you wish to proceed?"
        if [ $? -eq 1 ]; then
            echo "Exiting"
            exit 1
        fi
        rm -rf $christmaspi_install_location
    fi
    mkdir $christmaspi_install_location
    download_christmaspi
    run_christmaspi
}

main
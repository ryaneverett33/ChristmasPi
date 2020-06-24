class install {
    stepName = null;
    stepListObj = null;
    installStatus = null;

    constructor(installStep, stepList) {
        this.stepName = installStep;
        this.stepListObj = document.getElementById(stepList);
        this.installStatus = "none";
        // construct the listitem
    }
}
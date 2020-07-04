// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var currentColor;

function setActivePage(active) {
    switch (active) {
        case "Home":
            if ($('#li-nav-solid').hasClass('active'))
                $('#li-nav-solid').removeClass('active');
            if ($('#li-nav-animation').hasClass('active'))
                $('#li-nav-animation').removeClass('active');
            $('#li-nav-home').addClass('active');
            break;
        case "Animation":
            if ($('#li-nav-solid').hasClass('active'))
                $('#li-nav-solid').removeClass('active');
            if ($('#li-nav-home').hasClass('active'))
                $('#li-nav-home').removeClass('active');
            $('#li-nav-animation').addClass('active');
            break;
        case "Solid":
            if ($('#li-nav-animation').hasClass('active'))
                $('#li-nav-animation').removeClass('active');
            if ($('#li-nav-home').hasClass('active'))
                $('#li-nav-home').removeClass('active');
            $('#li-nav-solid').addClass('active');
            break;
    }
}
$(function () {
    $('#cp2').colorpicker({
        inline: true,
        container: false,
        format: "rgb",
        useAlpha: false,
        customClass: 'colorpicker-2x',
        sliders: {
            saturation: {
                maxLeft: 200,
                maxTop: 200
            },
            hue: {
                maxTop: 200
            },
            alpha: {
                maxTop: 200
            }
        }
    });
    $('#cp2').on('colorpickerChange colorpickerCreate', function (event) {
        currentColor = event.color.toString();
    });
    $("#power-checkbox").bootstrapSwitch();
    $('#power-checkbox').on('switchChange.bootstrapSwitch', function (e, data) {
        if (data)
            TurnOn();
        else
            TurnOff();
    });
    $('#brightness-slider').on('input', function (e) {
        var ammount = e.target.value;
        var percent = Math.round((ammount / 255) * 100);
        $("#brightnessvalue").text(`${percent}%`);
    });
});
function showErrorModal(errorMessage) {
    $("#genericErrorModalMessage").text(errorMessage);
    $('#genericErrorModal').modal();
}
function ShowAddRuleModal() {
    $("#addrule-starttime").datetimepicker({
        format: 'LT'
    });
    $("#addrule-endtime").datetimepicker({
        format: 'LT',
        icons: {
            time: "fa fa-clock-o",
            date: "fa fa-calendar",
            up: "fa fa-chevron-up",
            down: "fa fa-chevron-down",
            previous: 'fa fa-chevron-left',
            next: 'fa fa-chevron-right',
            today: 'fa fa-screenshot',
            clear: 'fa fa-trash',
            close: 'fa fa-remove'
        }
    });
    $('#ScheduleAddModal').modal();
}
function closeAddRuleModal() {
    $('#ScheduleAddModal').modal('hide');
}

function ShowRemoveRuleModal(i, j) {
    $('#scheduleremove-i').val(i);
    $('#scheduleremove-j').val(j);
    $('#ScheduleRemoveModal').modal();
}
function closeRemoveRuleModal() {
    $('#scheduleremove-i').val("");
    $('#scheduleremove-j').val("");
    $('#ScheduleRemoveModal').modal('hide');
}

function SetColor(colorstring) {
    $("#cp2").colorpicker('setValue', colorstring);
}

function SubmitColor() {
    // get color
    var color = currentColor;
    console.log(color);
    // /api/solid/update?color=db7b26
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to set color");
        }
        else {
            location.reload();
        }
    });
    //oReq.open("POST", "/api/solid/update?color=" + color);
    oReq.open("POST", "/api/solid/update");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send(JSON.stringify({ color: color }));
}
function PlayAnimation(animation) {
    // /api/animations/play?animation=Twinkle
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to play animation");
        }
        else {
            location.reload();
        }
    });
    // oReq.open("POST", "/api/animations/play?animation=" + animation);
    oReq.open("POST", "/api/animations/play");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send(JSON.stringify({ animation: animation }));
}
function ResumeAnimation() {
    // /api/animations/play
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to play animation");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/animations/play");
    oReq.send();
}
function PauseAnimation() {
    // /api/animations/pause
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to pause animation");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/animations/pause");
    oReq.send();
}
function StopAnimation() {
    // /api/animations/stop
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to stop animation");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/animations/stop");
    oReq.send();
}
function TurnOn() {
    // /api/power/on
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to turn on");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/power/on");
    oReq.send();
}
function TurnOff() {
    // /api/power/on
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to turn on");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/power/off");
    oReq.send();
}

function checkAddRuleDays() {
    var monday = $("#monday-check:checked").val() === "on";
    var tuesday = $("#tuesday-check:checked").val() === "on";
    var wednesday = $("#wednesday-check:checked").val() === "on";
    var thursday = $("#thursday-check:checked").val() === "on";
    var friday = $("#friday-check:checked").val() === "on";
    var saturday = $("#saturday-check:checked").val() === "on";
    var sunday = $("#sunday-check:checked").val() === "on";
    if (monday || tuesday || wednesday || thursday || friday || saturday || sunday)
        $("#scheduleAddRuleBtn").prop('disabled', false);
    else
        $("#scheduleAddRuleBtn").prop('disabled', true);
}

// seealso ChristmasPi.Data.Models.Scheduler.RepeatUsage
var REPEATMONDAY = 1;
var REPEATTUESDAY = 2;
var REPEATWEDNESDAY = 4;
var REPEATTHURSDAY = 8;
var REPEATFRIDAY = 16;
var REPEATSATURDAY = 32;
var REPEATSUNDAY = 64;

function getRepeatUsage() {
    var monday = $("#monday-check:checked").val() === "on";
    var tuesday = $("#tuesday-check:checked").val() === "on";
    var wednesday = $("#wednesday-check:checked").val() === "on";
    var thursday = $("#thursday-check:checked").val() === "on";
    var friday = $("#friday-check:checked").val() === "on";
    var saturday = $("#saturday-check:checked").val() === "on";
    var sunday = $("#sunday-check:checked").val() === "on";

    var mask = 0;
    if (monday)
        mask = mask | REPEATMONDAY;
    if (tuesday)
        mask = mask | REPEATTUESDAY;
    if (wednesday)
        mask = mask | REPEATWEDNESDAY;
    if (thursday)
        mask = mask | REPEATTHURSDAY;
    if (friday)
        mask = mask | REPEATFRIDAY;
    if (saturday)
        mask = mask | REPEATSATURDAY;
    if (sunday)
        mask = mask | REPEATSUNDAY;

    return mask;
}

function RemoveRule() {
    // /api/schedule/remove
    var i = parseInt($("#scheduleremove-i").val());
    var j = parseInt($("#scheduleremove-j").val());
    closeRemoveRuleModal();
    //var start = $(`#rstart-${i}:${j}`).val();
    var start = document.getElementById(`rstart-${i}:${j}`).value;
    //var end = $(`#rend-${i}:${j}`).val();
    var end = document.getElementById(`rend-${i}:${j}`).value;
    //var day = $(`#rday-${i}:${j}`).val();
    var day = document.getElementById(`rday-${i}:${j}`).value;

    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to remove rule");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/schedule/remove");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send(JSON.stringify({
        start: start,
        end: end,
        day: day
    }));
}

function AddRule() {
    // /api/schedule/add
    var start = $('#addrule-starttime').data("DateTimePicker").viewDate().format("HH:mm");
    var end = $('#addrule-endtime').data("DateTimePicker").viewDate().format("HH:mm");
    var repeat = getRepeatUsage();
    closeAddRuleModal();

    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to add rule");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/schedule/add");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send(JSON.stringify({
        start: start,
        end: end,
        repeat: repeat
    }));
}

function StartSetup() {
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to start setup");
        }
        else {
            location.href='/setup/next?current=start';
        }
    });
    oReq.open("POST", "/setup/start");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send();
}

function next() {
    var pathKeys = location.pathname.split('/');
    var current = pathKeys[pathKeys.length - 1];
    location.href=`/setup/next?current=${current}`;
}

function ValidateLights() {
    // light count can't be zero
    var lightcount = $("#lightcount").val();
    var maxstr = $("#lightcount").prop("max");
    try {
        var count = parseInt(lightcount);
        var max = parseInt(maxstr);
        if (count === 0 || count > max) {
            $("#lightcount").addClass("error");
            $("#submitbtn").prop("disabled", true);
        }
        else {
            $("#lightcount").removeClass("error");
            $("#submitbtn").prop("disabled", false);
        }
    }
    catch {
        $("#lightcount").addClass("error");
        $("#submitbtn").prop("disabled", true);
    }
}

var maxLightCount = 0;
var branches = null;
var currentBranchIndex = 0;
var working = false;            // if talking to the server

function getCurrentLightCount() {
    var lightCount = 0;
    for (var i = 0; i < branches.length; i++) {
        var branch = branches[i];
        lightCount = lightCount + branch.lightCount;
    }
    return lightCount;
}

function branchSetupLoad() {
    if (working)
        return;
    var oReq = new XMLHttpRequest();
    currentLightCount = 1;
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to add light");
            working = false;
        }
        else {
            var json = JSON.parse(this.response);
            branches = [];
            currentBranchIndex = 0;
            maxLightCount = parseInt($("#lightcount").val());
            branches.push(new branch(1, 0, "branchlist", json["color"]));
            validateLights();
            working = false;
        }
    });
    oReq.open("POST", "/setup/branches/branch/new");
    working = true;
    oReq.send();
}

function addLight() {
    if (working)
        return;
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to add light");
            working = false;
        }
        else {
            var branch = branches[currentBranchIndex];
            branch.add();
            validateLights();
            working = false;
        }
    });
    oReq.open("POST", "/setup/branches/light/new");
    working = true;
    oReq.send();
}

function minusLight() {
    if (working)
        return;
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to remove light");
            working = false;
        }
        else {
            var branch = branches[currentBranchIndex];
            branch.remove();
            validateLights();
            working = false;
        }
    });
    oReq.open("POST", "/setup/branches/light/remove");
    working = true;
    oReq.send();
}

function newBranch() {
    if (working)
        return;
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to create branch");
            working = false;
        }
        else {
            working = false;
            var json = JSON.parse(this.response);
            // get end of current branch
            var end = branches[currentBranchIndex].end;
            branches.push(new branch(end + 1, branches.length, "branchlist", json["color"]));
            currentBranchIndex = currentBranchIndex + 1;
            branches[currentBranchIndex - 1].deactivate();
            branches[currentBranchIndex].activate();
            validateLights();
        }
    });
    oReq.open("POST", "/setup/branches/branch/new");
    working = true;
    oReq.send();
}

function deleteBranch() {
    if (working)
        return;
    if (branches.length === 1)
        return;
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to delete branch");
            working = false;
        }
        else {
            branches[currentBranchIndex].delete();
            branches.pop();
            currentBranchIndex = currentBranchIndex - 1;
            branches[currentBranchIndex].activate();
            working = false;
            validateLights();
        }
    });
    oReq.open("POST", "/setup/branches/branch/remove");
    working = true;
    oReq.send();
}


function validateLights() {
    var start = branches[currentBranchIndex].start;
    var end = branches[currentBranchIndex].end;
    if (end <= start)
        $("#minusBtn").prop('disabled', true);
    else
        $("#minusBtn").prop('disabled', false);
    if (getCurrentLightCount() === maxLightCount) {
        $("#addBtn").prop('disabled', true);
        $("#newBtn").prop('disabled', true);
        $("#submitBtn").prop('disabled', false);
    }
    else {
        $("#addBtn").prop('disabled', false);
        $("#newBtn").prop('disabled', false);
        $("#submitBtn").prop('disabled', true);
    }
    // update light count
    $("#lightcounter").text(`${getCurrentLightCount()}/${maxLightCount}`);
}

function submitBranches() {
    if (working)
        return;
    if (getCurrentLightCount() !== maxLightCount) {
        showErrorModal("Not all lights have been assigned to a branch");
        return;
    }
    branchArray = [];
    for (var i = 0; i < branches.length; i++) {
        var branch = branches[i];
        branchArray.push({
            start: branch.start,
            end: branch.end
        });
    }
    var json = JSON.stringify(branches);
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to submit branches");
            working = false;
        }
        else {
            location.href = '/setup/next?current=branches';
        }
    });
    oReq.open("POST", "/setup/branches/submit");
    oReq.setRequestHeader("Content-Type", "application/json");
    working = true;
    oReq.send(json);
}

servicePoller = null;

function setServiceTitle(isMainService, status) {
    if (isMainService) {
        var title = `ChristmasPi Service - ${status}`
        $("#main-service-status").text(title);
    }
    else {
        var title = `Scheduler Service - ${status}`
        $("#scheduler-service-status").text(title);
    }
}

var savedText = null;
var successInstall = false;
var doInstallScheduler = false;

// set current text
function updateText(text) {
    newText = text;
    if (savedText != null)
        newText = savedText + '\n' + text;
    $("#programoutput").text(newText);
}

// save text from previous installation
function saveText(text) {
    savedText = text;
}

function installService(installScheduler) {
    doInstallScheduler = installScheduler;
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to start installation process");
        }
        else {
            if (!installScheduler)
                setServiceTitle(false, "Not Installing");
            $("#installprogress").show();
            servicePoller = setInterval(servicesInstallPoller, 500);
        }
    });
    oReq.open("POST", "/setup/services/install");
    oReq.setRequestHeader("Content-Type", "application/json");
    var data = {
        installScheduler : installScheduler
    };
    var json = JSON.stringify(data);
    oReq.send(json);
}

function servicesInstallPoller() {
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to get installation progress");
            clearInterval(servicePoller);
        }
        else {
            var obj = JSON.parse(this.responseText);
            switch (obj["status"]) {
                case "Stale":
                    break;
                case "Installing":
                    updateText(obj["output"]);
                    break;
                case "Success":
                    saveText(obj["output"]);
                    updateText(obj["output"]);
                    successInstall = true;
                    setServiceTitle(true, "Installed");
                    if (doInstallScheduler)
                        setServiceTitle(false, "Installing");
                    break;
                case "Failure":
                    clearInterval(servicePoller);
                    $("#continue-btn").show();
                    updateText(obj["output"]);
                    setServiceTitle(!successInstall, "Failed");
                    break;
                case "AllDone":
                    clearInterval(servicePoller);
                    if (doInstallScheduler)
                        setServiceTitle(false, "Installed");
                    else
                        setServiceTitle(true, "Installed");
                    $("#continue-btn").show();
                    break;
            }
        }
    });
    oReq.open("GET", "/setup/services/progress");
    oReq.send();
    // clearInterval(servicePoller);
}
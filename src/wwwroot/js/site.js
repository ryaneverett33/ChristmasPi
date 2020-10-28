// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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
function getRuleNameParts(rulename) {
    if (rulename == null || rulename == undefined)
        return null;
    if (rulename.match(/d[0-9]{1,2}s[1-9]{1,2}e[1-9]{1,2}/) == null)
        return null;
    var day = rulename.match(/d[1-9]{1,2}/)[0].substring(1);
    var start = rulename.match(/s[0-9]{1,2}/)[0].substring(1);
    var end = rulename.match(/e[0-9]{1,2}/)[0].substring(1);
    return {
        day: day,
        start: start,
        end: end
    }
}
function showErrorModal(errorMessage) {
    $("#genericErrorModalMessage").text(errorMessage);
    $('#genericErrorModal').modal();
}

/*function next() {
    var pathKeys = location.pathname.split('/');
    var current = pathKeys[pathKeys.length - 1];
    location.href=`/setup/next?current=${current}`;
}*/

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

var activeHoverClass = "";
var lastHoverClass = "";
// Used by schedule to draw shadows
function updateMouseHover(className) {
    // className should either be none or a rule-cell class
    lastHoverClass = activeHoverClass;
    activeHoverClass = className === "none" ? "" : className;
    addEventListener();
    updateShadows(); 
}
function addEventListener() {
    // rule-cell divs aren't responding to onclick for some reason so register the event listener if it's not already registered
    if (activeHoverClass === "")
        return;
    var element = document.getElementsByClassName(activeHoverClass)[0];
    if (element.onclick == null) {
        element.onclick = function() {
            var className = activeHoverClass;          // TODO would very much like to "hardcode" this value into the function 
            // example d1s2e3 and d1s17e23
            var partObjs = getRuleNameParts(className);
            if (partObjs == null)
                return;
            ShowRemoveRuleModal(partObjs.day, partObjs.start);
        };
    }
}
function updateShadows() {
    // deactivate lastHoverClass
    if (lastHoverClass !== "") {
        var elements = document.getElementsByClassName(lastHoverClass);
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            element.classList.remove("rule-cell-hovered");
        }
    }
    // activateHoverClass
    if (activeHoverClass !== "") {
        var elements = document.getElementsByClassName(activeHoverClass);
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            if (!element.classList.contains("rule-cell-hovered"))
                element.classList.add("rule-cell-hovered");
        }
    }
}
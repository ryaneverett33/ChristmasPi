class branch {
    start = 0;
    end = 0;
    count = 0;
    listObj = null;
    branchObj = null;
    branchid = 0;
    branchColor = null;

    constructor(start, branchid, listid, color) {
        this.start = start;
        this.end = start;
        this.branchid = branchid;
        this.branchColor = color;
        this.listObj = document.getElementById(listid);
        if (this.listObj === null)
            throw "Invalid listid, object is null";
        // create branch in DOM
        var li = document.createElement("LI");
        var h4 = document.createElement("H4");
        var liDiv = document.createElement("DIV");
        var colorDiv = document.createElement("DIV");
        var deleteBtn = document.createElement("BUTTON");
        var icon = document.createElement("I");
        // setup h4
        h4.id = `branch${branchid}`;
        h4.innerText = `Lights ${this.start}-${this.end}`;
        h4.classList.add("h-no-newline");
        // setup liDiv
        liDiv.classList.add("branchitem");
        // setup colorDiv
        colorDiv.classList.add("sm-colorbox");
        colorDiv.style.backgroundColor = color;
        // setup deletebtn
        //deleteBtn.appendChild(document.createTextNode("Delete"));
        deleteBtn.onclick = deleteBranch;
        deleteBtn.classList.add("btn");
        deleteBtn.classList.add("btn-info");
        deleteBtn.classList.add("btn-just-icon");
        // setup icon
        icon.classList.add("nc-icon");
        icon.classList.add("nc-simple-remove");
        deleteBtn.appendChild(icon);


        // add to DOM
        liDiv.appendChild(colorDiv);
        liDiv.appendChild(h4);
        liDiv.appendChild(deleteBtn);
        li.appendChild(liDiv);
        this.listObj.appendChild(li);
        this.branchObj = li;
    }

    // increments light count by one
    add() {
        this.end = this.end + 1;
        this.updateText();
    }
    // decrements light count by one
    remove() {
        this.end = this.end - 1;
        this.updateText();
    }

    // stops showing branch actions
    deactivate() {
        // disable delete button
        this.branchObj.childNodes[0].childNodes[2].style.display = "none";
    }

    // start showing branch actions
    activate() {
        // enable delete button
        this.branchObj.childNodes[0].childNodes[2].style.display = "inline-block";
    }

    updateText() {
        $(`#branch${this.branchid}`).text(`Lights ${this.start}-${this.end}`);
    }

    // deletes the branch from DOM
    delete() {
        this.listObj.removeChild(this.branchObj);
    }

    get lightCount() {
        if (this.end === this.start)
            return 1;
        else
            return (this.end - this.start) + 1;
    }
}
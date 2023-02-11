
const redScoreIncrease = document.querySelector(".Plus-Red");
const redScoreDecrease = document.querySelector(".Minus-Red");
const blueScoreIncrease = document.querySelector(".Plus-Blue");
const blueScoreDecrease = document.querySelector(".Minus-Blue");
const redScoreDisplay = document.querySelector(".Points-Red");
const blueScoreDisplay = document.querySelector(".Points-Blue");
const start = document.getElementById('start');
const reset = document.getElementById('reset');
const minute = document.getElementById("minute");
const second = document.getElementById("second");
const pause = document.getElementById("pause");
const play = document.getElementById("play");
const sendButtons = document.querySelectorAll("send");
const scoreRed = document.getElementsByClassName("scoreRed");
const scoreBlue = document.getElementsByClassName("scoreBlue");
const startMinutes = document.getElementsByClassName("startMinutes");
const startSeconds = document.getElementsByClassName("startSeconds");
const endMinutes = document.getElementsByClassName("endMinutes");
const endSeconds = document.getElementsByClassName("endSeconds");
const decision = document.getElementsByClassName("decision");
const points = document.getElementsByClassName("points");
let blueScore = 0;
let redScore = 0;
let isPaused = false;


redScoreIncrease.addEventListener('click', () => {

    redScore++;
    redScoreDisplay.textContent = redScore.toString();

    for (let i = 0; i < scoreRed.length; i++) {
        scoreRed[i].value = redScore;
    }
})

redScoreDecrease.addEventListener('click', () => {

    if (redScore > 0) {
        redScore--;
        redScoreDisplay.textContent = redScore.toString();
    }

    for (let i = 0; i < scoreRed.length; i++) {
        scoreRed[i].value = redScore;
    }
})

blueScoreIncrease.addEventListener('click', () => {

    blueScore++;
    blueScoreDisplay.textContent = blueScore.toString();

    for (let i = 0; i < scoreBlue.length; i++) {
        scoreBlue[i].value = blueScore;
    }
})

blueScoreDecrease.addEventListener('click', () => {

    if (blueScore > 0) {
        blueScore--;
        blueScoreDisplay.textContent = blueScore.toString();
    }

    for (let i = 0; i < scoreBlue.length; i++) {
        scoreBlue[i].value = blueScore;
    }
})

start.addEventListener('click', function startFunction () {

    function startInterval() {
        startGong();
        startTimer = setInterval(function () {
            if (!isPaused) {
                timer();
            }

            if (minute.value == 0 && second.value == 0) {
                stopInterval();
                isPaused = false;
            }

            leadingZeros(minute);
            leadingZeros(second);

        }, 1000);

        //Start Minutes
        for (let i = 0; i < startMinutes.length; i++) {
            startMinutes[i].value = minute.value;
        }

        //Start Seconds
        for (let i = 0; i < startSeconds.length; i++) {
            startSeconds[i].value = second.value;
        }
    }   
    startInterval();
    start.setAttribute('disabled', '');
})

reset.addEventListener('click', function () {
    minute.value = 0;
    second.value = 0;
    redScore = 0;
    blueScore = 0;
    redScoreDisplay.textContent = redScore.toString();
    blueScoreDisplay.textContent = blueScore.toString();
    stopInterval()
    isPaused = false;
    start.removeAttribute('disabled');
    for (let i = 0; i < decision.length; i++) {
        decision[i].setAttribute('disabled', '');
    }
    for (let i = 0; i < points.length; i++) {
        points[i].setAttribute('disabled', '');
    }
})

function timer() {
    if (minute.value == 0 && second.value == 0) {
        minute.value = 0;
        second.value = 0;
    } else if (second.value != 0) {
        second.value--;
    } else if (minute.value != 0 && second.value == 0) {
        second.value = 59;
        minute.value--;
    }
    if (second.value % 30 == 0 && !(minute.value == 0 && second.value == 0)) {
        highBeep();
        setTimeout(highBeep, 500);
    }
    if (minute.value == 0 && second.value == 0) {
        endGong();
    }

    if (minute.value == 0 && second.value == 0) {

        if (redScore == blueScore) {
            for (let i = 0; i < decision.length; i++) {
                decision[i].removeAttribute('disabled');
            }
        } else {
            if (blueScore > redScore) {
                document.getElementById("blue-by-points").removeAttribute('disabled');
            }

            if (redScore > blueScore) {
                document.getElementById("red-by-points").removeAttribute('disabled');
            }
        }
    }

    //End Minutes
    for (let i = 0; i < endMinutes.length; i++) {
        endMinutes[i].value = minute.value;
    }

    //End Seconds
    for (let i = 0; i < endSeconds.length; i++) {
        endSeconds[i].value = second.value;
    }

    return;
}

function stopInterval() {
    clearInterval(startTimer);
}

pause.addEventListener('click', function () {
    lowBeep();
    setTimeout(lowBeep, 500);
    isPaused = true;
})

play.addEventListener('click', function () {
    lowBeep();
    isPaused = false;
})

function leadingZeros(input) {
    if (!isNaN(input.value) && input.value.length === 1) {
        input.value = '0' + input.value;
    }
}

function startGong() {
    document.getElementById('start-gong').play();
}
function highBeep() {
    document.getElementById('high-beep').play();
}
function endGong() {
    document.getElementById('end-gong').play();
}
function lowBeep() {
    document.getElementById('low-beep').play();
}










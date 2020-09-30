// Problem from Tammy: what are all the possible combinations of moves
// a single player could make in three consecutive games of Rock Paper Scissors.
let args = process.argv;
let n = (args.slice(2)[0]);

if (n == undefined) {
    console.log("Provide a number of games to get the outcomes for.");
    return;
}

let actions = "RPS";
let totalRows = actions.length ** n;

console.log(`All combinations of the possible RPS moves a single player could make in ${n} consecutive games (${totalRows} possibilities.)`);

let numInARow = totalRows / actions.length;
var current = "";
let stateMachine = [];

while (numInARow >= 1) {

    let actionsToRepeat = [];

    for (var i = 0; i < actions.length; i++) {
        let currentAction = actions[i];
        actionsToRepeat.push({val: currentAction, count: numInARow});
    }

    stateMachine.push(actionsToRepeat);

    numInARow /= actions.length;
}

let finalRows = [];

stateMachine.forEach(states => {
    let current = "";
    while (current.length < totalRows) {
        states.forEach(state => {
            for (var i = 0; i < state.count; i++) {
                current += state.val;
            }
        });
    }

    finalRows.push(current);
    current = "";
});

// Results can be read vertically in the following.
console.log(finalRows);

// Transpose to output the results horizontally.
current = "";
for (var i = 0; i < totalRows; i++) {
    for (var j = 0; j < finalRows.length; j++) {
        current += finalRows[j][i];
    }

    console.log(current);
    current = "";
}
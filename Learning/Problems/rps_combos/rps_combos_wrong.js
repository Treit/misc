// Problem from Tammy: what are all the possible outcomes of three games of
// Rock Paper Scissors.
var args = process.argv;
console.log(args[1]);
var n = (args.slice(2)[0]);

console.log(process.argv[0]);

if (n == undefined) {
    console.log("Provide a number of games to get the outcomes for.");
    return;
}

var outcomes = getAllPossibleOutcomes();

console.log("Possible RPS outcomes:");

for (var i = 0; i < n; i++) {
    outcomes.forEach(x => console.log(x));
}

var outcomes = getAllPossibleOutcomes();

function getAllPossibleOutcomes() {
    var vals = "RPS";
    var answer = [];

    for (var i = 0; i < vals.length; i++) {
        for (var j = 0; j < vals.length; j++) {
            var item = vals[i] + vals[j];
            answer.push(item);
        }
    }

    return answer;
}

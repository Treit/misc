// Another problem from Tammy: is one array a subsequence of another array?
if (process.argv.length < 4) {
    console.log("Provide two arrays to compare.");
    return;
}

let a = process.argv[2];
let b = process.argv[3];

if (b.length > a.length) {
    console.log("The second array cannot be longer than the first.");
    return;
}

let result = isSubsequence(a, b);

console.log(result);

function isSubsequence(a, b) {
    let result = false;
    let currentStart = 0;

    while (result == false) {

        while (a[currentStart] != b[0] && currentStart < a.length) {
            currentStart++;
            continue;
        }

        if (currentStart == a.length) {
            break;
        }

        result = true;

        for (var i = 1; i < b.length; i++) {
            if (a[currentStart + i] != b[i]) {
                result = false;
                currentStart++;
                break;
            }
        }
    }

    return result;
}
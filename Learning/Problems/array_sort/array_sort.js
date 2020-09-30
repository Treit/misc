var a = [];

a.push(1);
a.push(2);
a.push(3);
a.push(10);
a.push(20);
a.push(5);

var a = a.sort((a, b) => {
    if (a > b) {
        return -1;
    } else if (a == b) {
        return 0;
    }
    
    return 1;
}).slice(0, 3).reduce((acc, curr) => acc * curr);

console.log(a);


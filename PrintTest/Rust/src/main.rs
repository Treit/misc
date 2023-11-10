use std::time::{Instant};
fn main() {
    let instant = Instant::now();
    for i in 0..1000 {
        println!("Iteration {}", i);
    }

    println!("Time elapsed: {:?} ms.", instant.elapsed().as_millis());
}

use std::time::Instant;
use std::io::{stdout, Write};

fn main() {
    let mut lock = stdout().lock();
    let instant = Instant::now();
    for i in 0..1000 {
        writeln!(lock, "Iteration {}", i).unwrap();
    }
    println!("Time elapsed: {:?} ms.", instant.elapsed().as_millis());
}

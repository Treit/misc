from timeit import time

start = timeit()
for i in range(1000):
    print(f"Iteration {i}")
end = timeit()

print(f"{(end - start) * 1000}ms")
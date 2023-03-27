## When to use an AOT approach:
#### An AOT approach should be used when:
- Memory and CPU power need to be used optimally during runtime due to limited resources (On mobile devices)
- When the performance is highest priority of an app, ensuring low-latency which is needed by real-time systems for example.
- Ensuring highest Security due to the code being run ahead-of-time, preventing runtime attacks that might exploit the system.

## When to use a JIT approach:
#### A JIT approach should be used when:
- When the app is running on dynamic environments like web apps for example, similar to AOT it reduces memory usage and increases performance, however on systems with long runtime, AOT will always be superior in the end.
- Rapid Prototyping: When needed, changed code can be seen quicker than using the AOT approach since JIT provides a faster dev. cycle.
- Reduced memory footprint: JIT compilers can optimize the code  at runtime, generating machine code only of the needed parts of the code. AOT generates code for the entire program, including unused code parts.
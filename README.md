# Iron Software Coding Challenge - Old Phone Pad

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![Tests](https://img.shields.io/badge/tests-100%25-success)

## Overview

This solution is designed to demonstrate **Clean Architecture**, **Robust Logic**, and **Test-Driven Development (TDD)**. It features a separate core library, a comprehensive test suite, and an interactive Console Application with a retro Nokia-style interface.

### Features
* **Core Logic:** A stateless, O(N) algorithm handles key cycling and backspaces.
* **Robustness:** Handles edge cases like null inputs, complex backspacing, and missing termination characters.
* **Interactive UI:** A custom ASCII-art interface simulates a real phone screen.

## Project Structure

The solution follows a modular "Clean Architecture" approach:

```text
IronPhonePad
├── IronPhonePadApp (Console)   # The Entry Point & UI
├── IronPhonePadBenchmarks      # The Imperical O(N) Proof And General Benchmarks
├── PhonePadLibrary (Class Lib) # The Pure Business Logic (No side effects)
└── PhonePadTests (xUnit)       # The Quality Assurance Suite
```

## Engineering Decisions

### 1. Algorithm: State Machine
Instead of nested `if/else` statements, the solution uses a **State Machine** approach.
* **Why:** This allows us to track the current button state and cycle count independently of the loop.
* **Complexity:**
    * **Time:** `O(N)` - We iterate through the input string exactly once.
    * **Space:** `O(N)` - We use a single `StringBuilder` buffer proportional to the output.

### 2. Memory Efficiency
`StringBuilder` is used instead of string concatenation. In C#, strings are immutable; repeated concatenation creates unnecessary garbage. `StringBuilder` allows mutable operations (like backspacing) without memory overhead.

### 3. Maintainability (SOLID)
* **Single Responsibility:** The `PhoneKeypad` class does one thing: Decode. It does not handle UI or I/O.
* **No Magic Strings:** Control keys (`#`, `*`) and mappings are defined as constants/readonly structures, making it easy to reconfigure (e.g., if "Send" changed to `@`).

## How to Run

### Run the Interactive App
For the best experience (to prevent visual artifacts), run the app in an **External Terminal** (PowerShell/CMD).

```bash
dotnet run --project IronPhonePadApp
```

* **Commands:**
    * Type digits `2-9` to enter text.
    * Type `*` to backspace.
    * Type `clear` or `cls` to wipe the screen.
    * Type `exit` to quit.

### Run the Tests
To verify the logic against the provided examples (including the Turing test case):

```bash
dotnet test
```

## Performance Benchmarks

The solution was benchmarked using **BenchmarkDotNet**.

**Environment:** .NET 9.0, Intel Core i7-1165G7

| Method            | Mean      | Gen0   | Allocated |
|------------------ |----------:|-------:|----------:|
| **Decode_Short** | 43.01 ns  | 0.0216 |     136 B |
| **Decode_Complex**| 67.72 ns  | 0.0229 |     144 B |
| **Decode_Stress** | 66.81 ns  | 0.0229 |     144 B |

### Key Takeaways
1.  **Nanosecond Execution:** The logic executes in under 70 nanoseconds, making it suitable for high-throughput systems (provided that the "n" (input size) remains within practical limits).
2.  **Minimal Allocation:** Memory usage is strictly controlled (~144 bytes per call), putting near-zero pressure on the Garbage Collector (GC).
3.  **Early Termination:** The stress test confirms that the algorithm efficiently halts processing immediately upon encountering the send key (`#`), regardless of remaining input size.

### Graph Analysis:
The scatter plot above visually confirms the **Linear Time Complexity (O(N))** of the solution.

As the **Sample Length** increases (vertical axis), the execution **Time** (horizontal axis) grows in direct proportion. The grouping of data points along the diagonal indicates a stable, predictable algorithm with minimal variance. There is no exponential curve, proving that the solution scales efficiently even as input sizes reach 10,000+ characters.

![O(N) Graph](https://i.ibb.co/v4x0WwYM/complexity-proof.png)



### AI Usage:

**Tool:** Google Gemini Gem

**Gem link:** https://gemini.google.com/gem/10a4QrtI4OZvE-1UjPfwqTvJJPhN-BRt6?usp=sharing

**What is a Gem?** A Gem is a custom-configured version of Gemini with a persistent "System Instruction." For this project, the Gem acted as an architectural supervisor.

In this project, the Gem was programmed with strict architectural guardrails, utilizing a Human-in-the-loop approach to ensure every AI-generated suggestion was validated against the project requirements.

**Strategy:** I defined the Gem with a system instruction that prioritized SOLID principles, State-Machine logic over nested conditionals, and performance benchmarking.

**Gem instructions:**

**Role:** You are a Senior Principal C# Architect acting as a pair programmer. Your goal is to guide the user to create a production-grade solution for the "Old Phone Pad" coding challenge.

**Context:** The user is applying for a role at Iron Software and must demonstrate "production-ready code," "stability," and "professional engineering standards."

**The Challenge:** Implement `public static String OldPhonePad(string input)` which decodes numeric keypad input into text.

* **Mapping:** 2=ABC, 3=DEF, 4=GHI, 5=JKL, 6=MNO, 7=PQRS, 8=TUV, 9=WXYZ.
* **Special Keys:** * `*` is Backspace (deletes the last committed character).
  * `#` is Send (terminates the input).
  * `(space)` is a pause (commits the currently cycling character).
* **Logic:** Repeated presses cycle letters on the same button. A different button press or a space commits the current letter.

**Your Deliverables:**
1.  **Project Structure:** Recommend a clean .NET console app structure with a separate Class Library and xUnit Test Project.
2.  **Code Quality:** Enforce SOLID principles. Avoid "magic strings." Use constants or strict mapping.
3.  **Algorithm:** Suggest a state-machine or buffer-based approach rather than nested `if/else` statements to ensure robustness.
4.  **Testing:** Provide comprehensive unit tests covering edge cases (null, empty, only #, complex backspacing).
5.  **Documentation:** Draft a professional `README.md` explaining the approach, complexity (Big O), and how to run tests.

**Tone:** Professional, critical, and educational. If the user suggests sloppy code, politely correct them with a "Senior Engineer" perspective.

**Format:** Use Markdown for code blocks. Provide C# code that is clean, refactored, and commented.




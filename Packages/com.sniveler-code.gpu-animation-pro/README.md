# 🚀 GPU Animation Entities PRO

**The ultimate high-performance crowd animation system for Unity DOTS / ECS.**

Render **10,000+ animated units** at maximum FPS with zero CPU bottlenecks. Built specifically for the Universal Render Pipeline (URP), this asset bypasses traditional animation overhead by baking skeletal data into highly optimized **BlobAssets** and performing matrix/Dual Quaternion skinning directly inside the Vertex Shader.

Whether you are building a massive RTS, a factory automation game, or a swarm-survival (Vampire Survivors-like) project, **GPU Animation Entities PRO** provides AAA-level features with ECS-native simplicity.

---

## ✨ Why Choose PRO?

### ⚔️ Zero-Latency Sockets (Attachments)
Unlike traditional GPU skinning where weapons often lag by a frame, our **Burst-compiled synchronous lookup** allows you to attach swords, armor, or VFX to specific bones with **frame-perfect precision**.

### 🎨 Full Shader Graph Support
Don't limit yourself to basic shaders. We provide out-of-the-box **Shader Graph templates**. Add dissolve effects, cel-shaded outlines, or custom emission logic to your animated crowds in seconds.

### 🦾 Dual Quaternion Skinning (DQS)
Say goodbye to the "candy-wrapper" effect where joints (shoulders, knees) lose volume during extreme bending. **DQS preserves mesh volume**, drastically improving the visual fidelity of stylized and low-poly characters.

### ⚙️ One-Click Automated Baking
No complex setup. Drag your standard Unity GameObject (with a `SkinnedMeshRenderer` and `Animator`) into our custom Editor Window, click **"Bake"**, and receive a fully configured DOTS Prefab ready to be spawned via ECS.

### 🚶‍♂️ True Root Motion
Extract movement data directly from your animations. Our system applies **Root Motion deltas synchronously**, allowing your characters to move exactly as the animator intended, fully compatible with custom DOTS physics.

### 🚀 Extreme Performance & LODs
Built-in support for Unity's `LODGroup`. The system automatically scales bone influences (**4 bones → 2 bones → 1 bone**) and disables interpolation for distant units and shadow casters to save ALU instructions.

---

## 📦 What's Included?

* **The Animator Baker Window:** A powerful, tabbed visual editor for configuring LODs, extracting Root Motion, and exposing bones for Sockets.
* **Prefab Animation Settings:** A visual timeline editor to preview animations and set up weapon equip/unequip events.
* **Shader Graph Templates:** Ready-to-use Lit and Unlit templates with DOTS animation logic pre-wired.
* **Clean C# ECS API:** Fluent builder patterns to control animations and parameters via code without string allocations.
* **3 Massive Demo Scenes:**
    * **Demo 1:** Basics of baking and UI-driven animation control.
    * **Demo 2:** Showcasing the Zero-Latency Socket System with weapon equipping.
    * **Demo 3:** A massive RTS stress test featuring spatial hashing, combat decision-making, and thousands of units.

---

## 🛠️ Technical Requirements

| Requirement | Version |
| :--- | :--- |
| **Unity Version** | 2022.2+ (Unity 6 Supported) |
| **Render Pipeline** | URP 14.0+ (HDRP/Built-in not supported) |
| **Entities (DOTS)** | 1.4.5+ |
| **Entities Graphics** | 1.4.18+ |
| **Burst Compiler** | 1.8.27+ |

> [!IMPORTANT]
> This asset relies strictly on the **ECS paradigm**. It does not use traditional GameObjects for runtime rendering. You must be familiar with spawning and managing entities via `EntityCommandBuffer` or Bakers.

---

## 📚 Documentation & Support

We believe in zero-friction development. Comprehensive documentation is available to guide you from your first FBX to your first 10,000 units.

📖 [[Read the Full Online Documentation Here]](https://sniveler-code.gitbook.io/dots/gpu-animation-entities-pro)

### Need help?
* 📧 **Email:** [sniveler.code@gmail.com](mailto:sniveler.code@gmail.com)
* 🌐 **GitHub:** [sniveler-code](https://github.com/sniveler-code)

---
*Developed with ❤️ for high-performance Unity developers.*

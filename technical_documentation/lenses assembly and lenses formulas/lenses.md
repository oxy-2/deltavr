# DeltaVR - dual fresnel optics

> [!summary] summary
> d.70 pmma fresnels, **2 mm** thick each. stack is **f40 (display) + f30 (eye)**, grooves side meeting at the middle seperated by the gasket (*0.2mm*)
> combined efl **≈ 17.2 mm**. display gap is **15.0 mm**. eye relief is **20 mm** design (15–22 band). 10 mm is lashes-on-glass and the nose will not fit.

**parts:** d70 fl30 (top) + d70 fl40(bottom), 5 € ish each also buy spares. 

**cut outline:** see `deltavr-lens-cut.png` (optical center on pupil, nose bite, mirror L/R). trim does **not** change focal length (fl).

---

## 1. layout

```
display active surface
   │
   |  15.0 mm air          <-- dist. to disp. from len not holder, thats 14mm
   ▼
   f40 fresnel  2 mm       flat side toward display, grooves inward
   │
   │  0.2 mm spacer        printed as first layer (ring, not take up fov too much)
   ▼
   f30 fresnel  2 mm       grooves inward, flat toward eye <- top is more focused
   │
   │  20 mm air            eye relief (15–22 band, 25 if glasses)
   ▼
cornea of eye
```

| gap                   | value        | notes                                                           |
| --------------------- | ------------ | --------------------------------------------------------------- |
| display -> f40 flat   | **15.0 mm**  | caliper this. 14 = near focus, 15 = infinity-ish, 16 = far bias |
| f40 thickness         | **2.0 mm**   | catalog, cat. shouldnt change                                   |
| between lenses        | **0.2 mm**   | grooves face each other. ring spacer, not a disc                |
| f30 thickness         | **2.0 mm**   | catalog                                                         |
| f30 flat -> eye       | **20 mm**    | design. 15–22 band. 25 if glasses. 10 = lashes + nose smash     |
| **optical module**    | **19.2 mm**  | display face → outer face of eye lens. cylinder depth           |
| **display -> cornea** | **~39.2 mm** | includes eye-relief air. do **not** put this in the barrel      |

---

## 2. sign convention

real-is-positive, thin-lens form:

$$\frac{1}{f} = \frac{1}{d_o} + \frac{1}{d_i}$$

virtual image (eyepiece case) → $d_i < 0$.

---

## 3. single lens

| symbol | meaning | value |
|---|---|---|
| $f$ | focal length (efl) | 30 mm or 40 mm |
| $d_o$ | object distance (display → lens) | solve |
| $d_i$ | image distance (lens → image) | $-\infty$ or $-250\ \mathrm{mm}$ |
| $t$ | substrate thickness | **2.0 mm** |
| $n$ | pmma index | 1.49 (catalog fl already includes this) |

> [!note] fresnel fl reference
> catalog fl is measured from the **grooved surface**. the 2 mm blank is mechanical dead plastic; power lives on the grooves. that is why $g_{\mathrm{disp}} = \mathrm{FFL} - t$.

---

## 4. two thin lenses, separation $d$

groove-to-groove: $d = 0.2\ \mathrm{mm}$.

$f_1 = 40$ (display side), $f_2 = 30$ (eye side).

### equivalent focal length

$$\frac{1}{f_{\mathrm{eq}}} = \frac{1}{f_1} + \frac{1}{f_2} - \frac{d}{f_1 f_2}$$

$$\frac{1}{f_{\mathrm{eq}}} = \frac{1}{40} + \frac{1}{30} - \frac{0.2}{40 \cdot 30} = 0.025 + 0.033333 - 0.000167 = 0.058167$$

$$\boxed{f_{\mathrm{eq}} = 17.19\ \mathrm{mm}}$$

contact limit ($d \approx 0$):

$$f = \frac{f_1 f_2}{f_1 + f_2} = \frac{1200}{70} = 17.14\ \mathrm{mm}$$

---

## 5. front focal length (display placement)

object at front focal point → image at infinity (relaxed eye). ffl is measured from the **first lens groove plane** ($L_1$ = f40).

$$\mathrm{FFL} = f_{\mathrm{eq}}\left(1 - \frac{d}{f_2}\right) = 17.19 \times 0.99333 = 17.08\ \mathrm{mm}$$

principal-plane offset from $L_1$ is $\bar{H_1} = -f_{\mathrm{eq}}\, d / f_2 \approx -0.12\ \mathrm{mm}$. negligible. treat the f40 groove plane as the front reference.

**mechanical gap (flat → display):**

$$\boxed{g_{\mathrm{disp}} = \mathrm{FFL} - t_1 = 17.08 - 2.0 = 15.1\ \mathrm{mm} \approx 15.0\ \mathrm{mm}}$$

---

## 6. focus target (not just infinity)

$$\frac{1}{d_o} = \frac{1}{f_{\mathrm{eq}}} - \frac{1}{d_i}, \qquad d_i = -|d_i|\ \text{(virtual)}$$

$$d_o = \left(\frac{1}{f_{\mathrm{eq}}} + \frac{1}{|d_i|}\right)^{-1}$$

$$\boxed{g_{\mathrm{disp}}(d_i) = \left(\frac{1}{f_{\mathrm{eq}}} + \frac{1}{|d_i|}\right)^{-1} - t_1}$$

| target image | $\|d_i\|$ | $d_o$ (to groove) | $g_{\mathrm{disp}} = d_o - t_1$ |
|---|---|---|---|
| infinity (relaxed) | $\infty$ | 17.1 mm | **15.1 mm** |
| 2 m (far) | 2000 | 16.4 mm | 14.4 mm |
| 25 cm (reading) | 250 | 16.1 mm | **14.1 mm** |
| 10 cm (too close) | 100 | 14.4 mm | 12.4 mm |
| 6 cm (why 13.2 hurts) | 58 | 13.2 mm | 11.2 mm |

> [!warning] old 13.2 mm gap
> manolo's measured 13.2 mm doesnt match 30+40 thin lens math. it puts the virtual image at ~6 cm and eyes will hate it. if a housing is already cut at 13.2, print a ~2 mm shim. design point is **15.0 mm**, shim range **14–16 mm**.

---

## 7. eye side

firstly, eye relief is mechanical not optical:

$$g_{\mathrm{eye}} = \mathrm{cornea} - \mathrm{f30\ flat}$$

| use                      | $g_{\mathrm{eye}}$ |
| ------------------------ | ------------------ |
| lashes / fantasy         | 8–10 mm            |
| aggressive nose trim     | 15–18 mm           |
| **design (fits a face)** | **20 mm**          |
| glasses / easy fit       | 22–25 mm           |

10 mm is eyelashes on the flat. the nose also will not fit: nose tip sits ~25–45 mm in front of the cornea, so at 10 mm relief the bridge is still deep inside the lens volume. 20–25 mm is the real mechanical floor for a full ø70 stack and a normal nose. closer than that needs a molded face interface almost nobody will print right on try one.

first-order fov does **not** care about relief. relief buys eye box and clearance.

$$r_{\mathrm{clear}} \gtrsim g_{\mathrm{eye}} \cdot \tan(\mathrm{FOV}/2) + \mathrm{eye\ box}/2 + r_{\mathrm{pupil}}$$

at fov 75°/axis (tan 38° ≈ 0.78) and a 5 mm eye box:

| $g_{\mathrm{eye}}$ | $r_{\mathrm{clear}}$ needed | note |
|---|---|---|
| 10 mm | ~16 mm | easy, but lashes |
| 20 mm | ~23 mm | keep this radius on the **temple + top** |
| 25 mm | ~27 mm | almost untrimmed ø70, nose dies |

so: do **not** shrink the whole lens. cut a **nose bite only** (manolo kidney / d-shape). keep r = 35 mm on the outer, top, and bottom-temporal edges. inboard at pupil height, clear down to ~18 mm from the optical center (≈13 mm from midline at 63 ipd). below pupil height the bite opens more for the nose body.

inboard eye box will be tighter than outboard. that is normal. lose a little binocular overlap before you lose temple fov.

---

## 8. full stack length

### optical module

ends at the outer face of the eye lens, **not** at the cornea.

$$L_{\mathrm{mod}} = g_{\mathrm{disp}} + t_1 + d + t_2$$

$$L_{\mathrm{mod}} = 15.0 + 2.0 + 0.2 + 2.0 = \mathbf{19.2\ \mathrm{mm}}$$

### display → cornea

$$\boxed{L_{\mathrm{total}} = g_{\mathrm{disp}} + t_1 + d + t_2 + g_{\mathrm{eye}} = 15.0 + 2.0 + 0.2 + 2.0 + 20.0 = 39.2\ \mathrm{mm}}$$

| $g_{\mathrm{eye}}$ | $L_{\mathrm{total}}$ display→cornea | housing $W_{\mathrm{FB}}$ (approx) |
|---|---|---|
| 15 mm | 34.2 mm | ~44 mm |
| **20 mm (design)** | **39.2 mm** | **~49 mm** |
| 25 mm | 44.2 mm | ~54 mm |

quest 2 body is ~48 mm. 20 mm relief puts us at quest-class thickness, not "thin". the thin goal dies here unless relief or the optical module shrinks. do not fake it with 10 mm.

### housing front-back (mechanical)

$$W_{\mathrm{FB}} = t_{\mathrm{display\ PCB}} + t_{\mathrm{cover}} + L_{\mathrm{mod}} + g_{\mathrm{eye}} + t_{\mathrm{face\ gasket}}$$

$$\approx 5 + 1 + 19.2 + 20 + 4 \approx \mathbf{49\ \mathrm{mm}} \quad (\sim 45\ \mathrm{mm}\ \text{if pcb+gasket squeezed})$$

> [!danger] dont double count
> do not put eye relief inside the optical cylinder. module is 19.2 mm. the 20 mm of air in front of the eye is outside the barrel.

> [!warning] nose bridge
> no solid bar between the lens bottoms. the nose has to climb into the open gap after the nose bite. route structure over the top and along the temples. face gasket gets a deep nose cutout that matches the lens bite.

---

## 9. fov

panel is the LS029B3SX02, active $w = 26.6\ \mathrm{mm}$, half-width $h = 13.3\ \mathrm{mm}$.

### ideal

$$\mathrm{FOV}_{1\mathrm{D}} = 2 \arctan\left(\frac{h}{f_{\mathrm{eq}}}\right) = 2 \arctan\left(\frac{13.3}{17.19}\right) = \mathbf{75.4°}$$

diagonal (square panel, half-diag $= h\sqrt{2}$):

$$\mathrm{FOV}_{\mathrm{diag}} = 2 \arctan\left(\frac{h\sqrt{2}}{f_{\mathrm{eq}}}\right) = 2 \arctan\left(\frac{18.81}{17.19}\right) = \mathbf{95.2°}$$

general:

$$\boxed{\mathrm{FOV} = 2 \arctan\left(\frac{d_{\mathrm{sensor}}}{2\, f_{\mathrm{eq}}}\right)}$$

### real (relief + diameter losses)

relief does not change the 75°/axis first-order number. it changes whether your pupil can sit in the ray bundle (eye box) and how hard the nose bite clips the inboard field.

$$r_{\mathrm{clear}} \gtrsim g_{\mathrm{eye}} \cdot \tan(\mathrm{FOV}/2) + \mathrm{eye\ box}/2 + r_{\mathrm{pupil}}$$

keep r ≥ 23 mm on temple/top at 20 mm relief. nose bite can go down to ~18 mm from the optical center inboard. expect **5–10° less** on the diagonal after nose bite + a little eye-box loss. still quest 2-class (~75° / ~95° ideal).

---

## 10. alternate stack (two f30s)

only real shrink lever. same blank, same cutout, one part number.

$$f_{\mathrm{eq}} = \frac{30 \cdot 30}{30 + 30} = 15.0\ \mathrm{mm}$$

$$g_{\mathrm{disp}} = 15.0 - 2.0 = 13.0\ \mathrm{mm}$$

$$L_{\mathrm{total}} = 13 + 2 + 0.2 + 2 + 20 = 37.2\ \mathrm{mm}$$

$$\mathrm{FOV}_{1\mathrm{D}} = 2\arctan(13.3/15) = 82.0°$$

| stack | efl | display gap | display→cornea ($g_{\mathrm{eye}}=20$) | fov / axis |
|---|---|---|---|---|
| **f40 + f30** (current) | 17.2 mm | 15.0 mm | 39.2 mm | 75° |
| f30 + f30 | 15.0 mm | 13.0 mm | 37.2 mm | 82° |

f30+f30 buys +7°/axis and 2 mm less path. it does not fix the nose. nose is trim + gasket + open nose bridge.

---

## 11. diameter (keep ø70)

matching diameters with **both** fl30 and fl40 in the listing:

| diameter | fl30 | fl40 | both? |
|---|---|---|---|
| **d70** | yes | **yes** | **only option** |
| d90 | yes | (fl50) | no |
| d100 | | yes | no |
| d60 | yes | (fl80) | no |
| d50 / d40 | no | no | no |

d100 fl120 at 10 € is a trap. cutout work stays valid on ø70.

---

## 12. variable sheet

| variable | symbol | formula | value |
|---|---|---|---|
| display-side fl | $f_1$ | catalog | 40 mm |
| eye-side fl | $f_2$ | catalog | 30 mm |
| lens thickness | $t_1, t_2$ | catalog | 2.0 mm each |
| spacer | $d$ | first layer | 0.2 mm |
| combined efl | $f_{\mathrm{eq}}$ | $\left(\frac{1}{f_1}+\frac{1}{f_2}-\frac{d}{f_1 f_2}\right)^{-1}$ | **17.19 mm** |
| front focal length | ffl | $f_{\mathrm{eq}}(1 - d/f_2)$ | 17.08 mm |
| display gap (mech) | $g_{\mathrm{disp}}$ | $\mathrm{FFL} - t_1$ | **15.0 mm** |
| focus target | $d_i$ | design choice | $\infty$ or $-250\ \mathrm{mm}$ |
| object distance | $d_o$ | $\left(\frac{1}{f_{\mathrm{eq}}}+\frac{1}{\|d_i\|}\right)^{-1}$ | 17.1 / 16.1 mm |
| eye relief | $g_{\mathrm{eye}}$ | mechanical | **20 mm** (15–22) |
| optical module | $L_{\mathrm{mod}}$ | $g_{\mathrm{disp}}+t_1+d+t_2$ | **19.2 mm** |
| display→cornea | $L_{\mathrm{total}}$ | $L_{\mathrm{mod}}+g_{\mathrm{eye}}$ | **39.2 mm** |
| half panel | $h$ | $w/2$ | 13.3 mm |
| fov / axis | $\mathrm{FOV}_{1\mathrm{D}}$ | $2\arctan(h/f_{\mathrm{eq}})$ | **75°** |
| fov diagonal | $\mathrm{FOV}_{\mathrm{diag}}$ | $2\arctan(h\sqrt{2}/f_{\mathrm{eq}})$ | **95°** |
| diameter | $D$ | keep cutout | 70 mm |
| ior | $n$ | pmma | 1.49 |

---

## 13. what is *not* in the math

- **$t = 2\ \mathrm{mm}$ does not change $f$.** fresnel power is the groove prism angles. thickness only enters as $g_{\mathrm{disp}} = \mathrm{FFL} - t$ because catalog fl is from the grooves.
- **cutting the outline does not change $f$.** trim is mechanical only.
- **thick-lens principal-plane shift** $\bar{H} \sim t/n \approx 1.3\ \mathrm{mm}$ matters for bulk lenses. for a fresnel with all power at one face, the principal plane sits at the grooves. already handled.
- **seidel aberrations, distortion, astigmatism.** not first-order. the pair (stronger toward the eye) trims some spherical aberration vs a single element; it does not change the first-order numbers above.

---

## 14. build rules

1. optical center (groove center) stays on the pupil.
2. grooves meet in the middle; flats face display and eye.
3. f30 (shorter fl, "more focused") goes toward the **eye**. f40 toward the **display**.
4. spacer is a ø70 **ring** (od 70, id = clear aperture). nothing plastic over the optical zone.
5. no soldering iron on faces. acrylic cement on the outer rim only, or a mechanical ring cassette. heat warps grooves and the lens is dead.
6. mirror the cut outline for left/right. hand-trim is the known-good path.
7. shim the display gap. 15.0 mm nominal, 14–16 mm is the useful window.
8. **nose bite only**, do not shrink the whole blank. keep r = 35 mm outer/top. inboard at pupil height cut to ~18 mm from optical center. below pupil the bite opens for the nose body. see `deltavr-lens-cut.png`.
9. face gasket gets a deep nose cutout matching the bite. no solid nose bridge bar between the lens bottoms.
10. eye relief 20 mm nominal. 15 mm if the trim is aggressive and the face is narrow. 25 mm for glasses. never 10 mm.

---

## 15. summary

$$f_{\mathrm{eq}} = 17.2\ \mathrm{mm} \implies g_{\mathrm{disp}} = f_{\mathrm{eq}} - 2 = 15.0\ \mathrm{mm}$$

$$L_{\mathrm{total}} = 15+2+0.2+2+20 = 39.2\ \mathrm{mm}, \quad \mathrm{FOV} \approx 75°/95°$$

eye relief 20 mm (15–22). 10 mm is lashes and no nose. fov is set by display / efl, not by relief. relief buys nose clearance and eye box. cut a nose bite, keep the rest of the ø70.

---

*deltavr optics · 2 mm pmma · ø70 · f40+f30 · spacer 0.2 mm*

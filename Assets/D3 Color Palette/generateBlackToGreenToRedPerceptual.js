import { PNG } from "pngjs";
import fs from "fs";
import { lab, rgb } from "d3-color";

const width = 256;
const height = 1;

const png = new PNG({
  width,
  height,
});

const anchors = [
  rgb(0, 0, 0),      // black
  rgb(0, 90, 0),     // dark green
  rgb(255, 0, 0),    // pure red
].map((c) => lab(c));

function lerp(a, b, t) {
  return a + (b - a) * t;
}

function labAt(segmentIndex, t) {
  const a = anchors[segmentIndex];
  const b = anchors[segmentIndex + 1];
  return lab(
    lerp(a.l, b.l, t),
    lerp(a.a, b.a, t),
    lerp(a.b, b.b, t)
  );
}

function deltaEApprox(c1, c2) {
  const dl = c2.l - c1.l;
  const da = c2.a - c1.a;
  const db = c2.b - c1.b;
  return Math.sqrt(dl * dl + da * da + db * db);
}

// Build a cumulative perceptual-distance map along black->green->red.
const sampleCount = 4096;
const path = [];
let cumulative = 0;
let prev = labAt(0, 0);
path.push({ cumulative, segmentIndex: 0, t: 0 });

for (let i = 1; i <= sampleCount; i++) {
  const u = i / sampleCount;
  const segmentPosition = u * 2;
  const segmentIndex = Math.min(1, Math.floor(segmentPosition));
  const t = segmentPosition - segmentIndex;
  const current = labAt(segmentIndex, t);
  cumulative += deltaEApprox(prev, current);
  path.push({ cumulative, segmentIndex, t });
  prev = current;
}

const totalDistance = cumulative;

for (let x = 0; x < width; x++) {
  const target = (x / (width - 1)) * totalDistance;

  let lo = 0;
  let hi = path.length - 1;
  while (lo < hi) {
    const mid = Math.floor((lo + hi) / 2);
    if (path[mid].cumulative < target) {
      lo = mid + 1;
    } else {
      hi = mid;
    }
  }

  const p = path[lo];
  const c = labAt(p.segmentIndex, p.t).rgb();

  const idx = x * 4;
  png.data[idx] = Math.max(0, Math.min(255, Math.round(c.r)));
  png.data[idx + 1] = Math.max(0, Math.min(255, Math.round(c.g)));
  png.data[idx + 2] = Math.max(0, Math.min(255, Math.round(c.b)));
  png.data[idx + 3] = 255;
}

png.pack().pipe(fs.createWriteStream("blackToGreenToRedPerceptual.png"));

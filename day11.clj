(ns adventofcode.day9
  (:require [adventofcode.util :as u]
            [adventofcode.day5 :as cpu]))

(def blank-memory (cpu/create-memory (u/input-csv 11)))

(def blank-canvas {[0 0] \e})

(def direction {0 [0 1] 180 [0 -1] 90 [-1 0] 270 [1 0]})

(def paint-color {0 \b 1 \w})
(def color->instruction {\b 0 \w 1 nil 0 \e 0})

(defn get-color [canvas coord]
  (if (contains? canvas coord) (canvas coord) \b))

(defn run-cpu [mem input idx ridx len]
  (loop [cpu-output (cpu/solve-interuptable mem input idx ridx) n 1 output {:memory [] :painter-input []}]
    (if (or (= len n) (nil? (first cpu-output)))
      (assoc (assoc output :memory (rest cpu-output)) :painter-input (conj (output :painter-input) (first cpu-output)))
      (recur (cpu/solve-interuptable (second cpu-output) input (nth cpu-output 2) (last cpu-output))
             (inc n) (assoc output :painter-input (conj (output :painter-input) (first cpu-output)))))))

(defn change-direction [dir ins]
  (case ins 0 (mod (- dir 90) 360) 1 (mod (+ dir 90) 360)))

(defn +v [[w x] [y z]]
  [(+ w y) (+ x z)])

(defn run-painter [painter-memory paint move]
  (let [painted-canvas (assoc (painter-memory :canvas) (painter-memory :coordinates) (paint-color paint))
        new-dir (change-direction (painter-memory :direction) move)
        new-coord (+v (painter-memory :coordinates) (direction new-dir))]
    {:canvas painted-canvas
     :direction new-dir
     :coordinates new-coord
     :color-under (get-color painted-canvas new-coord)}))

(defn part-one []
  (loop [painter-memory {:canvas blank-canvas
                         :direction 0
                         :coordinates [0 0]
                         :color-under \b}
         cpu-memory [blank-memory 0 0]
         i 0]
    (let [cpu-output (run-cpu (first cpu-memory) (color->instruction (painter-memory :color-under)) (second cpu-memory) (last cpu-memory) 2)
          cpu-out-mem (cpu-output :memory) cpu-out-instructions (cpu-output :painter-input)]
      (println (painter-memory :color-under))
      (if (nil? (first cpu-out-instructions))
        (painter-memory :canvas)
        (recur (run-painter painter-memory (first cpu-out-instructions) (second cpu-out-instructions)) cpu-out-mem (inc i))))))

(println (count (part-one)))

(ns adventofcode.day8
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(def size [25 6])

(def width (first size))

(def height (second size))

(def color {0 :black 1 :white 2 :transparent})

(def input (slurp (u/path 8)))

(def layers (partition (reduce * size) input))

(def mlayers (map (partial partition width) layers))

(defn part-one []
  (let [c (map frequencies layers)
        n (map #(% \0) c)]
    (->> (apply min n)
         (.indexOf n)
         (nth c)
         (#(* (% \1) (% \2))))))

(defn part-two []
  (loop [c 0 li 0
         output (vec (repeat height []))]
    (if (= c (reduce * size))
      output
      (let [x (mod c width) y (int (/ c width))
            layer (nth mlayers li)
            pix (nth (nth layer y) x)]
        (if (= \2 pix)
          (recur c (inc li) output)
          (recur (inc c) 0 (assoc output y (conj (nth output y) pix))))))))

(time (part-one))
;; => 2684
;; => "Elapsed time: 0.134089 msecs"

(time (part-two))
;; =>
;[[\1 \0 \0 \0 \1 \0 \1 \1 \0 \0 \1 \1 \1 \0 \0 \1 \0 \0 \0 \1 \1 \1 \1 \1 \0]
; [\1 \0 \0 \0 \1 \1 \0 \0 \1 \0 \1 \0 \0 \1 \0 \1 \0 \0 \0 \1 \0 \0 \0 \1 \0]
; [\0 \1 \0 \1 \0 \1 \0 \0 \0 \0 \1 \0 \0 \1 \0 \0 \1 \0 \1 \0 \0 \0 \1 \0 \0]
; [\0 \0 \1 \0 \0 \1 \0 \1 \1 \0 \1 \1 \1 \0 \0 \0 \0 \1 \0 \0 \0 \1 \0 \0 \0]
; [\0 \0 \1 \0 \0 \1 \0 \0 \1 \0 \1 \0 \1 \0 \0 \0 \0 \1 \0 \0 \1 \0 \0 \0 \0]
; [\0 \0 \1 \0 \0 \0 \1 \1 \1 \0 \1 \0 \0 \1 \0 \0 \0 \1 \0 \0 \1 \1 \1 \1 \0]]
;;=> "Elapsed time: 22.170338 msecs"

;(map println (part-two))
;[1 0 0 0 1 0 1 1 0 0 1 1 1 0 0 1 0 0 0 1 1 1 1 1 0]
;[1 0 0 0 1 1 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 0 0 1 0]
;[0 1 0 1 0 1 0 0 0 0 1 0 0 1 0 0 1 0 1 0 0 0 1 0 0]
;[0 0 1 0 0 1 0 1 1 0 1 1 1 0 0 0 0 1 0 0 0 1 0 0 0]
;[0 0 1 0 0 1 0 0 1 0 1 0 1 0 0 0 0 1 0 0 1 0 0 0 0]
;[0 0 1 0 0 0 1 1 1 0 1 0 0 1 0 0 0 1 0 0 1 1 1 1 0]
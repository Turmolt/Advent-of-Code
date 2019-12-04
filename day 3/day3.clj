(ns adventofcode.day3
  (:require [clojure.string :as str]))

(defn abs [n] (max n (- n)))
(defn distance [[x y]] (+ (abs x) (abs y)))

(def input
  (map #(str/split % #",") (str/split (slurp "day 3/input.txt") #"\n")))

(defn movement [d]
  (case d
    \U [0 1]
    \D [0 -1]
    \R [1 0]
    \L [-1 0]))

(def instruction 
  (juxt first #(Integer. (re-find #"\d+" %))))

(defn create-line [path [d n]]
  (concat path
          (rest (reductions #(map + %1 %2) (last path) (repeat n (movement d))))))

(def wire (partial transduce (map instruction) (completing create-line)))

(let [wires (map #(rest (wire '((0 0)) %)) input)
      intersections (apply clojure.set/intersection (map set wires))]
  (println (apply min-key distance intersections))
  (println (+ (count wires)
              (->> intersections
                   (map (fn [v] (apply + (map #(.indexOf % v) wires))))
                   (apply min)))))
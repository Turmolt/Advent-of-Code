(ns adventofcode.day3
  (:require [clojure.string :as str]))

(defn abs [n] (max n (- n)))

(defn read-and-split [path]
  (map #(str/split % #",") (str/split (slurp path) #"\n")))

(defn between [i s f]
  (and (<= s i) (>= f i)))

(defn create-segment [s i]
  (let [d (Integer. (re-find #"\d+" i))]
    (case (str (first i))
      "U" [(first s) (+ (second s) d)]
      "D" [(first s) (- (second s) d)]
      "L" [(- (first s) d) (second s)]
      "R" [(+ (first s) d) (second s)]
      ))
  )

(defn create-line [ins]
  (loop [s [[[0 0]]]
         i 0]
    (if (> (count ins) i)
      s
      (recur (concat s [[(last (last s))(create-segment (last (last s)) (nth ins i))]]) (inc i))
      )))

(defn intersects [l1 l2]
  (let [p11 (first l1) p12 (second l1)
        p21 (first l2) p22 (second l2)
        x11 (first p11) x12 (first p12)
        y11 (second p11) y12 (second p12)
        x21 (first p21) x22 (first p22)
        y21 (second p21) y22 (second p22)
        ta1 (+ (* (- y21 y22) (- x11 x21)) (* (- x22 x21) (- y11 y21)))
        ta2 (- (* (- x22 x21) (- y11 y12)) (* (- x11 x12) (- y22 y21)))
        tb1 (+ (* (- y11 y12) (- x11 x21)) (* (- x12 x11) (- y11 y21)))
        tb2  (- (* (- x22 x21) (- y11 y12)) (* (- x11 x12) (- y22 y21)))]
    (if (or (= ta2 0) (= tb2 0))
      1000000000
      (let [ta (/ ta1 ta2)
            tb (/ tb1 tb2)]
        (if (and (between ta 0 1) (between tb 0 1))
          (do
            (println (str "x: " (+ x11 (* ta (- x12 x11))) "  y: " (+ y11 (* ta (- y12 y11)))))
            (+ (abs (+ x11 (* ta (- x12 x11)))) (abs (+ y11 (* ta (- y12 y11))))))
          1000000000)))
    ))

(defn create-and-intersect [i]
  (let [l1 (create-line (first i))
        l2 (create-line (second i))]
    (loop [closest 1000000
           idx 0]
      (if (and (< idx (count l1)) (< idx (count l2)))
        (let [l1s (nth l1 idx) l2s (nth l2 idx)
              dis (intersects l1s l2s)]
          (if (> closest dis)
            (do
              (println dis)
              (recur dis (inc idx)))
            (recur closest (inc idx))))
        (println closest)))))

(create-and-intersect (read-and-split "day 3/input.txt"))
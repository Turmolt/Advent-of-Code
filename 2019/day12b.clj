(ns adventofcode.2019.day12b
  (:require [adventofcode.util :as u]))

(def x-axis-s0 [[9 -3 -4 0] [0 0 0 0]])
(def y-axis-s0 [[13 16 11 -2] [0 0 0 0]])
(def z-axis-s0 [[-8 -17 -10 -2] [0 0 0 0]])

(defn apply-gravity [positions pos vel]
  (->> positions
       (map #(compare % pos))
       (reduce +)
       (+ vel)))

(defn calculate-velocity [pos vel]
  (map (partial apply-gravity pos) pos vel))

(defn time-step [[pos vel]]
  (let [new-vel (calculate-velocity pos vel)
        new-pos (map + pos new-vel)]
    [new-pos new-vel]))

(defn period [state]
  (+ 1 (.indexOf
        (->> state
             (iterate time-step)
             (rest))
        state)))

(defn part-two []
  (->> [x-axis-s0 y-axis-s0 z-axis-s0]
       (map period)
       (reduce u/lcm)
       (println)))

(part-two)

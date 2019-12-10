(ns adventofcode.day10
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(def asteroid-field (remove nil? (str/split-lines (u/input 10))))

(def station [31 20])

(defn calc-angle [[x1 y1] [x2 y2]]
  (->> (Math/atan2 (- y2 y1) (- x2 x1))
       (* (/ 180 Math/PI))))

(defn coord [width idx itm]
  [[(mod idx width) (int (/ idx width))] itm])

(defn map-field [field]
  (let [width (count (first field))]
    (->> (flatten field)
         (#(flatten (map vec %)))
         (map-indexed (partial coord width))
         (remove (fn [x] (= \. (second x))))
         (map first))))

(def asteroids (map-field asteroid-field))

(defn find-angle [coordinates start]
  (map (partial calc-angle start) (vec (remove (partial = start) coordinates))))

(defn find-angles [coordinates start]
  [start (find-angle coordinates start)])

(defn find-angles-indexed-distance [start coordinates]
  (let [xc (first coordinates) yc (second coordinates)
        xs (first start) ys (second start)
        distance (Math/sqrt (+ (Math/pow (- xc xs) 2) (Math/pow (- yc ys) 2)))]
    [(Math/abs distance) (calc-angle coordinates start) coordinates]))

(defn part-one []
  (->> asteroids
       (map (comp (fn [x] [(first x) (count (distinct (second x)))])
                  (partial find-angles asteroids)))
       (reduce (fn [x y] (if (> (second x) (second y)) x y)))))

;(time (part-one))
;; => [[31 20] 319]
;; => "Elapsed time: 206.7806 msecs"

(defn part-two []
  (map (partial find-angles-indexed-distance station) (vec (remove (partial = station) asteroids))))

(defn p2 []
  (map reverse (partition-by second (sort-by #(mod (- (second %) 90) 360) (part-two)))))

(nth (p2) 199)

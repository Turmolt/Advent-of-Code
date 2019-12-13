(ns adventofcode.day12
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(def input (->> (u/input 0)
                (#(str/replace % #"\<" ""))
                (#(str/replace % #"\>" ""))
                (str/split-lines)
                (map #(str/split % #","))
                (map (partial map (comp #(str/split % #"=") str/trim)))))

(defn position [target axis] ((target :position) axis))

(defn get-position [target] [(position target :x) (position target :y) (position target :z)])

(defn velocity [target axis] ((target :velocity) axis))

(defn get-velocity [target] [(velocity target :x) (velocity target :y) (velocity target :z)])

(defn raw->map [[x y z]] {:x x :y y :z z})

(defn create-moon [idx [x y z]]
  {:position {:x (read-string (second x)) :y (read-string (second y)) :z (read-string (second z))}
   :velocity {:x 0 :y 0 :z 0}
   :id idx})

(defn resume-moon [idx [x y z] [vx vy vz]]
  {:position {:x (read-string (second x)) :y (read-string (second y)) :z (read-string (second z))}
   :velocity {:x vx :y vy :z vz}
   :id idx})

(defn parse-into-moons [raw] (map-indexed create-moon raw))

(def parsed-moons (parse-into-moons input))

(defn calculate-velocity-delta [target other-moon]
  (let [dx (compare (position other-moon :x) (position target :x))
        dy (compare (position other-moon :y) (position target :y))
        dz (compare (position other-moon :z) (position target :z))]
    {:x dx :y dy :z dz}))

;this is yucky.. but i don't know a better clojure way to do it yet. reduce [[1 2] [1 2] [1 2]] => [3 6]
;if you see this and know of a better way (because of course clojure has a macro for this i just can't find it) id love for you to let me know
(defn reduce-velocity-delta [raw]
  (->> (mapcat identity raw)
       (sort-by first)
       (partition-by first)
       (map (partial map second))
       (map (partial reduce +))))

(defn update-velocity [target moon-list]
  (let [other-moons (remove #(= (target :id) (% :id)) moon-list)]
    (->> (map (partial calculate-velocity-delta target) other-moons)
         (reduce-velocity-delta)
         (mapv + (get-velocity target))
         (#(assoc target :velocity (raw->map %))))))

(defn update-position [target]
  (assoc target :position (raw->map (mapv + (get-position target) (get-velocity target)))))

(defn update-moon [moons target]
  (update-position (update-velocity target moons)))

(defn reduce-energy [moon]
  (* (+ (Math/abs (position moon :x)) (Math/abs (position moon :y)) (Math/abs (position moon :z)))
     (+ (Math/abs (velocity moon :x)) (Math/abs (velocity moon :y)) (Math/abs (velocity moon :z)))))

(defn part-one []
  (->> (loop [moons parsed-moons iterations 0]
         (if (>= iterations 1000) moons
             (recur (mapv (partial update-moon moons) moons) (inc iterations))))
       (map reduce-energy)
       (reduce +)))

(defn part-two [axis]
  (let [original (map #(list (position % axis) (velocity % axis)) parsed-moons)]
    (loop [moons parsed-moons iterations 0]
      (let [present (map #(list (position % axis) (velocity % axis)) moons)]
        (if (and (= original present) (> iterations 0)) iterations
            (recur (mapv (partial update-moon moons) moons) (inc iterations)))))))

;(map #(list (position % :z) (velocity % :z)) parsed-moons)

;(part-one)

;(part-two :x)
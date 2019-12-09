(ns adventofcode.day8
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(def size [25 6])

(def width (first size))

(def height (second size))

(def layers (partition (reduce * size) (u/input 8)))

(defn part-one []
  (let [c (map frequencies layers)
        n (map #(% \0) c)]
    (->> (apply min n)
         (.indexOf n)
         (nth c)
         (#(* (% \1) (% \2))))))

(defn part-two []
  (->> layers
       (reduce (partial map (fn [x y] (if (= \2 x) y x))))
       (partition width)
       (map (partial map #(if (= \0 %) " " %)))))

(time (part-one))
;; => 2684
;; => "Elapsed time: 3.837947 msecs"

(time (part-two))
;; =>
;((\1 " " " " " " \1 " " \1 \1 " " " " \1 \1 \1 " " " " \1 " " " " " " \1 \1 \1 \1 \1 " ")
; (\1 " " " " " " \1 \1 " " " " \1 " " \1 " " " " \1 " " \1 " " " " " " \1 " " " " " " \1 " ")
; (" " \1 " " \1 " " \1 " " " " " " " " \1 " " " " \1 " " " " \1 " " \1 " " " " " " \1 " " " ")
; (" " " " \1 " " " " \1 " " \1 \1 " " \1 \1 \1 " " " " " " " " \1 " " " " " " \1 " " " " " ")
; (" " " " \1 " " " " \1 " " " " \1 " " \1 " " \1 " " " " " " " " \1 " " " " \1 " " " " " " " ")
; (" " " " \1 " " " " " " \1 \1 \1 " " \1 " " " " \1 " " " " " " \1 " " " " \1 \1 \1 \1 " "))
;;=> "Elapsed time: 0.048916 msecs"

;(map println (part-two))
;(1       1   1 1     1 1 1     1       1 1 1 1 1  )
;(1       1 1     1   1     1   1       1       1  )
;(  1   1   1         1     1     1   1       1    )
;(    1     1   1 1   1 1 1         1       1      )
;(    1     1     1   1   1         1     1        )
;(    1       1 1 1   1     1       1     1 1 1 1  )
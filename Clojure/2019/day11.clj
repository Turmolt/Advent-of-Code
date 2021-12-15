(ns adventofcode.2019.day9
  (:require [adventofcode.util :as u]
            [adventofcode.2019.day5 :as cpu]))

(def blank-memory (cpu/create-memory (u/input-csv 2019 11)))

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

(defn run-painter [painter-memory [paint move]]
  (let [painted-canvas (assoc (painter-memory :canvas) (painter-memory :coordinates) (paint-color paint))
        new-dir (change-direction (painter-memory :direction) move)
        new-coord (mapv + (painter-memory :coordinates) (direction new-dir))]
    {:canvas painted-canvas
     :direction new-dir
     :coordinates new-coord
     :color-under (get-color painted-canvas new-coord)}))

(defn run-robot [start-color]
  (loop [painter-memory {:canvas blank-canvas
                         :direction 0
                         :coordinates [0 0]
                         :color-under start-color}
         cpu-memory [blank-memory 0 0]]
    (let [cpu-output (run-cpu (first cpu-memory)
                              (color->instruction (painter-memory :color-under))
                              (second cpu-memory)
                              (last cpu-memory) 2)]
      (if (some nil? (cpu-output :painter-input))
        (painter-memory :canvas)
        (recur (run-painter painter-memory (cpu-output :painter-input)) (cpu-output :memory))))))

(defn part-one []
  (->> (run-robot \b)
       (count)))

(defn part-two []
  (->> (run-robot \w)
       (sort-by #(first (first %)))
       (partition-by #(first (first %)))
       (map (partial sort-by first))
       (map (partial map second))
       (map (partial map #(if (= % \b) " " "#")))
       (reverse)
       (map println)))

;(time (part-one))
;; => 2041
;; => "Elapsed time: 1170.0913 msecs"

(time (part-two))
;; => (     )
;;    (# #       #)
;;    (#   #     #)
;;    (#     #   #)
;;    (#       # #)
;;    (           )
;;    (# # # # # #)
;;    (    #     #)
;;    (  # #     #)
;;    (#     # #  )
;;    (           )
;;    (# #       #)
;;    (#   #     #)
;;    (#     #   #)
;;    (#       # #)
;;    (           )
;;    (# # # # # #)
;;    (    #     #)
;;    (    #     #)
;;    (      # #  )
;;    (           )
;;    (# # # # # #)
;;    (      #    )
;;    (  # #   #  )
;;    (#         #)
;;    (           )
;;    (# # # # # #)
;;    (#     #   #)
;;    (#     #   #)
;;    (#         #)
;;    (           )
;;    (# #       #)
;;    (#   #     #)
;;    (#     #   #)
;;    (#       # #)
;;    (           )
;;    (# # # # # #)
;;    (    #     #)
;;    (  # #     #)
;;    (#     # #  )
;;    (           )
;;    (       )
;;    (   )
;; => "Elapsed time: 124.2382 msecs"

package barsimmon.colorreplacer.service;

import lombok.extern.log4j.Log4j2;
import org.springframework.stereotype.Service;

import javax.annotation.PostConstruct;
import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.Objects;

@Service
@Log4j2
public class ColorReplacerService {
    private static final String PATH = "/home/sam/modding/Rimworld/FantasyGoblinsUpdated/v1.4/Textures/Things/Goblin";
    private static final String OUTPUT = "/home/sam/modding/out/";
    public static final int LIGHTEST_IS_WHITE = 15;
    public static final int SECOND_LIGHTEST_IS_WHITE = 55;
    public static final double BROWN_TOLERANCE = 1.1;
    public static final int MAX_BLACK = 32;

    @PostConstruct
    public void init() throws IOException {
        Map<String, BufferedImage> images = getImages(PATH + "/Bodies");
        images.putAll(getImages(PATH + "/Heads"));

        for (Map.Entry<String, BufferedImage> image : images.entrySet()) {
            File output = new File(OUTPUT + image.getKey());
            replaceColor(image.getValue());
            ImageIO.write(image.getValue(), "png", output);
        }

        String wat = "wat";
    }

    private Map<String, BufferedImage> getImages(String dirPath) {
        Map<String, BufferedImage> images = new HashMap<>();
        File dir = new File(dirPath);
        if (dir.isDirectory()) {
            for (File file : Objects.requireNonNull(dir.listFiles())) {
                try {
                    images.put(file.getName(), ImageIO.read(file));
                } catch (IOException e) {
                    log.error("Can't read from file {}", file.getName());
                }
            }
        }
        return images;
    }

    private void replaceColor(BufferedImage image) {
        for (int x = 0; x < image.getWidth(); x++) {
            for (int y = 0; y < image.getHeight(); y++) {
                int rgb = image.getRGB(x, y);
                if (rgb != 0x00FFFFFF) {
                    int a = (rgb & 0xFF000000) >> 24;
                    int r = (rgb & 0x00FF0000) >> 16;
                    int g = (rgb & 0x0000FF00) >> 8;
                    int b = (rgb & 0x000000FF);

                    if ((greenIsBrightest(r, g, b) || isABrown(r, g, b))) {
                        int newGray;
                        if (isABlack(r, g, b)) {
                            newGray = Math.min(Math.min(r, g), 2);
                        } else {
                            newGray = Math.min(g + SECOND_LIGHTEST_IS_WHITE, 255);
                        }
                        int newValue = (a << 24) + (newGray << 16) + (newGray << 8) + newGray;
                        image.setRGB(x, y, newValue);
                    }
                }
            }
        }
    }

    private static boolean isABrown(int r, int g, int b) {
        return g > r / BROWN_TOLERANCE && b > 20;
    }

    private static boolean greenIsBrightest(int r, int g, int b) {
        return g > r && g > b;
    }

    private static boolean isABlack(int r, int g, int b) {
        return Math.max(Math.max(r, g), b) <= MAX_BLACK;
    }
}

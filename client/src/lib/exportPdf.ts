import html2canvas from "html2canvas";
import { jsPDF } from "jspdf";

export async function exportPdf(
  element: HTMLElement | null,
  filename: string = "export"
) {
  if (!element) return;

  const canvas = await html2canvas(element, {
    scale: 2,
    useCORS: true,
    logging: false,
  });

  const imgData = canvas.toDataURL("image/png");
  const imgWidth = canvas.width;
  const imgHeight = canvas.height;

  const pdfWidth = imgWidth * 0.75;
  const pdfHeight = imgHeight * 0.75;

  const pdf = new jsPDF({
    orientation: pdfWidth > pdfHeight ? "landscape" : "portrait",
    unit: "pt",
    format: [pdfWidth, pdfHeight],
  });

  pdf.addImage(imgData, "PNG", 0, 0, pdfWidth, pdfHeight);
  pdf.save(`${filename}.pdf`);
}

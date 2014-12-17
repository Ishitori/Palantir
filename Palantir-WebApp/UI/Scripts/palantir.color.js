(function()
{
   if (window.Palantir.Color == null)
   {
       window.Palantir.Color = Color;
   }

   function Color(r, g, b, a)
   {
       var red = r,
           green = g,
           blue = b,
           alpha = a,
           pThis = this;

       pThis.toHexString = function()
       {
           return ('rgba(' + red + ',' + green + ',' + blue + ',' + alpha + ');').toUpperCase();
       };

       pThis.addRed = function(value)
       {
           red = getNewValue(red, value);
       };

       pThis.addGreen = function(value)
       {
           green = getNewValue(green, value);
       };

       pThis.addBlue = function(value)
       {
           blue = getNewValue(blue, value);
       };
       
       function getNewValue(baseValue, increment)
       {
           if (increment < 0 && baseValue - increment < 0)
           {
               return 0;
           }

           if (increment > 0 && baseValue + increment > 255)
           {
               return 255;
           }

           return baseValue + increment;
       };
   }
}());